using GuitarTrainer.Dtos;

namespace GuitarTrainer.Services
{
    public interface ISampleExercise
    {
        public Task<List<SampleExerciseTaskDto>> GetSamplesForExerciseAsync();
        public Task<List<AnswerOptionDto>> GetAnswerOptionsAsync();
    }
}
