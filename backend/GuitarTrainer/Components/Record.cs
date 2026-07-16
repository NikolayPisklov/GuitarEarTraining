using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using NWaves.Operations;
using NWaves.Signals;

namespace GuitarTrainer.Components
{
    public class Record
    {
        private readonly float[] _samples;
        private readonly float[] _samplesIn16k;
        private readonly int _sampleRate;
        private readonly InferenceSession _session;

        public Record(float[] samples, int sampleRate) 
        {
            _samples = samples;
            _sampleRate = sampleRate;
            _samplesIn16k = ResampleTo16k();
            _session = new InferenceSession("CrepeModels/full.onnx");
        }
        public async Task<List<PitchPoint>> GetCentsValuesOverTimeAsync()
        {
            const int frameSize = 1024;
            const int hopSize = 160;

            string inputName = _session.InputMetadata.Keys.First();

            var result = new List<PitchPoint>();

            for (int offset = 0; offset + frameSize < _samplesIn16k.Length; offset += hopSize)
            {
                float[] frame = new float[frameSize];

                Array.Copy(_samplesIn16k, offset, frame, 0, frameSize);

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
        private double BinToAbsoluteCents(double bin)
        {
            return bin * 20.0 + 1997.3794084376191;
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
        private float[] ResampleTo16k()
        {
            if (_sampleRate == 16000)
            {
                return _samples;
            }
            var signal = new DiscreteSignal(_sampleRate, _samples);
            var resampled = Operation.Resample(signal, 16000);
            return resampled.Samples;
        }
    }
}
