using GuitarTrainer.Dtos;
using GuitarTrainer.Enums;
using GuitarTrainer.Model;
using Microsoft.EntityFrameworkCore;

namespace GuitarTrainer.Services
{
    public class ExerciseResultService : IExerciseResultService
    {
        private readonly AppDbContext _db;

        public ExerciseResultService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<double> GetScoreOfLastAttemptAsync(int exerciseId, Guid userId)
        {
            var score = await _db.Attempts
                .Where(a => a.ExerciseId == exerciseId && a.UserId == userId)
                .OrderByDescending(a => a.AttemptDate)
                .Select(a => a.Score)
                .FirstAsync();
            return score;
        }

        public async Task InsertAttemptAsync(List<bool> answers, int exerciseId, Guid userId)
        {
            int correctCount = answers.Count(a => a);
            double score = (correctCount * 100) / answers.Count;
            var attempt = new Attempt
            {
                UserId = userId,
                ExerciseId = exerciseId,
                AttemptDate = DateTime.UtcNow,
                Score = score
            };
            _db.Add(attempt);
            await _db.SaveChangesAsync();
        }
        public async Task InsertAttemptAsync(BendExerciseResultDto result, int exerciseId, Guid userId) 
        {
            double score;
            var deviation = Math.Abs(result.centsDeviation);
            score = Math.Clamp((15 - deviation) / 15.0 * 100, 0, 100);
            var attempt = new Attempt
            {
                UserId = userId,
                ExerciseId = exerciseId,
                AttemptDate = DateTime.UtcNow,
                Score = score
            };
            _db.Add(attempt);
            await _db.SaveChangesAsync();
        }
    }
}
