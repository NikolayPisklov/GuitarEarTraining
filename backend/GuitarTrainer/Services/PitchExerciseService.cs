using GuitarTrainer.Dtos;
using GuitarTrainer.Model;
using Microsoft.EntityFrameworkCore;

namespace GuitarTrainer.Services
{
    public class PitchExerciseService : ISampleExercise
    {
        private readonly AppDbContext _db;

        public PitchExerciseService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<SampleExerciseTaskDto>> GetSamplesForExerciseAsync()
        {
            var samples = await _db.Samples
                .Where(s => s.ExerciseId == (int)ExerciseEnum.Pitch)
                .OrderBy(x => Guid.NewGuid())
                .Take(5)
                .Select(s => new SampleExerciseTaskDto
                (
                    s.CorrectAnswerId,
                    s.CorrectAnswer.TitleCode,
                    s.Id,
                    s.Path
                )).ToListAsync();
            return samples;
        }
    }
}
