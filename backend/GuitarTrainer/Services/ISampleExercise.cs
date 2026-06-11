using GuitarTrainer.Dtos;

namespace GuitarTrainer.Services
{
    public interface ISampleExercise
    {
        public Task<List<SampleExerciseTaskDto>> GetSamplesForExerciseAsync();
    }
}
