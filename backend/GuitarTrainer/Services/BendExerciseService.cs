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
            var result = CreateBendExerciseResult(centDifference, bendCents);
            return result;
        }
        private BendExerciseResultDto CreateBendExerciseResult(double differenceInCents, double bendCents) 
        {
            double resultDifference = bendCents - differenceInCents;
            if (resultDifference <= 5.0 && resultDifference >= -5)//Almost not noticable difference in pitch
            {
                return new BendExerciseResultDto(true, resultDifference);
            }
            else if (resultDifference <= 10 && resultDifference >= -10)//Noticable for a trained ear
            {
                return new BendExerciseResultDto(true, resultDifference);
            }
            else if (resultDifference <= 15 && resultDifference >= -15)//Okay result 
            {
                return new BendExerciseResultDto(true, resultDifference);
            }
            else //Difference to the point, where the note is noticably placed between the semitones. Not Acceptable :) 
            {
                return new BendExerciseResultDto(false, resultDifference);
            }
        }
        private async Task<double> CalculateBendDifferenceInCentsAsync(IFormFile file) 
        {
            var points = await GetCentsValuesOverTimeAsync(file);
            var ordered = points
                .Select(p => p.RelativeCents)
                .OrderBy(x => x)
                .ToList();

            var min = ordered[(int)(ordered.Count * 0.05)];
            var max = ordered[(int)(ordered.Count * 0.95)];

            return max - min;
        }
        private async Task<List<PitchPoint>> GetCentsValuesOverTimeAsync(IFormFile file)
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

                var (bin, confidence) = GetWeightedBin(output);

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
        private static double BinToAbsoluteCents(double bin)
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
        private (double Bin, float Confidence) GetWeightedBin(Tensor<float> output)
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

            int start = Math.Max(0, bestBin - 4);
            int end = Math.Min(359, bestBin + 4);

            double weightedSum = 0;
            double weightSum = 0;

            for (int i = start; i <= end; i++)
            {
                double weight = output[0, i];

                weightedSum += i * weight;
                weightSum += weight;
            }

            double weightedBin = weightedSum / weightSum;

            return (weightedBin, bestValue);
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
