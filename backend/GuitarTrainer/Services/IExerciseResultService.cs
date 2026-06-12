namespace GuitarTrainer.Services
{
    public interface IExerciseResultService
    {
        public Task InsertAttemptAsync(List<bool> answers, int exerciseId, Guid userId);
        public Task<double> GetScoreOfLastAttemptAsync(int exerciseId, Guid userId);
    }
}
