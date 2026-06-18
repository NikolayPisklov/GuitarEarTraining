using GuitarTrainer.Components;
using GuitarTrainer.Dtos;
using GuitarTrainer.Enums;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace GuitarTrainer.Services
{
    public class BendExerciseService
    {
        private readonly InferenceSession _session;

        public BendExerciseService()
        {
            _session = new InferenceSession("CrepeModels/full.onnx");
        }
        public async Task<BendExerciseResultDto> ProcessUserBendFileAsync(IFormFile file, BendType bendType) 
        {
            var centDifference = await CalculateBendDifferenceInCentsAsync(file);
            var bendCents = (double)bendType;
            var result = FormBendExerciseResult(centDifference, bendCents);
            return result;
        }
        public BendExerciseResultDto FormBendExerciseResult(double differenceInCents, double bendCents) 
        {
            if (bendCents - differenceInCents <= 5.0)
            {
                return new BendExerciseResultDto(true, differenceInCents);
            }
            else if (bendCents - differenceInCents <= 10)
            {
                return new BendExerciseResultDto(true, differenceInCents);
            }
            else if (bendCents - differenceInCents <= 15)
            {
                return new BendExerciseResultDto(true, differenceInCents);
            }
            else
            {
                return new BendExerciseResultDto(false, differenceInCents);
            }
        }
        public async Task<double> CalculateBendDifferenceInCentsAsync(IFormFile file) 
        {
            var points = await GetCentsPointsOverTimeAsync(file);
            var maxPoint = points.Max(p => p.RelativeCents);
            var minPoint = points.Min(p => p.RelativeCents);
            return maxPoint - minPoint;
        }
        private async Task<List<PitchPoint>> GetCentsPointsOverTimeAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();

            var samples = ReadSamples16kMono(stream);

            const int frameSize = 1024;
            const int hopSize = 160;

            string inputName = _session.InputMetadata.Keys.First();

            var result = new List<PitchPoint>();

            for (int offset = 0; offset + frameSize < samples.Length; offset += hopSize)
            {
                float[] frame = new float[frameSize];

                Array.Copy(samples, offset, frame, 0, frameSize);

                var tensor = new DenseTensor<float>(frame, new[] { 1, frameSize });

                using var outputs = _session.Run(new[]
                {
                    NamedOnnxValue.CreateFromTensor(inputName, tensor)
                });

                var output = outputs.First().AsTensor<float>();

                var (bin, confidence) = GetBestBin(output);

                double cents = BinToAbsoluteCents(bin);

                double timeMs = offset * 1000.0 / 16000.0;

                if (confidence < 0.8f)
                {
                    continue;
                }

                result.Add(new PitchPoint(timeMs, cents, confidence));
            }

            return result;
        }
        private static double BinToAbsoluteCents(int bin)
        {
            return bin * 20.0 + 1997.3794084376191;
        }
        private static (int Bin, float Confidence) GetBestBin(Tensor<float> output)
        {
            int bestBin = 0;
            float bestValue = output[0, 0];

            for (int i = 1; i < 360; i++)
            {
                if (output[0, i] > bestValue)
                {
                    bestValue = output[0, i];
                    bestBin = i;
                }
            }

            return (bestBin, bestValue);
        }
        private float[] ReadSamples16kMono(Stream stream)
        {
            using var reader = new WaveFileReader(stream);

            ISampleProvider provider = reader.ToSampleProvider();

            if (provider.WaveFormat.Channels > 1)
            {
                provider = new StereoToMonoSampleProvider(provider);
            }

            if (provider.WaveFormat.SampleRate != 16000)
            {
                provider = new WdlResamplingSampleProvider(provider, 16000);
            }

            var samples = new List<float>();
            var buffer = new float[4096];

            int read;

            while ((read = provider.Read(buffer, 0, buffer.Length)) > 0)
            {
                for (int i = 0; i < read; i++)
                {
                    samples.Add(buffer[i]);
                }
            }

            var result = samples.ToArray();

            float maxAbs = result.Select(Math.Abs).Max();

            if (maxAbs > 0)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] /= maxAbs;
                }
            }

            return result;
        }
    }
}
