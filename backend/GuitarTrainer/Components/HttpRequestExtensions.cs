using GuitarTrainer.Exceptions;

namespace GuitarTrainer.Components
{
    public static class HttpRequestExtensions
    {
        public static async Task<Record> GetRecordFromBody(this HttpRequest request, int sampleRate) 
        {
            using var memoryStream = new MemoryStream();
            await request.Body.CopyToAsync(memoryStream);
            var bytes = memoryStream.ToArray();
            var samples = new float[bytes.Length / sizeof(float)];
            Buffer.BlockCopy(bytes, 0, samples, 0, bytes.Length);
            return new Record(samples, sampleRate);
        }
    }
}
