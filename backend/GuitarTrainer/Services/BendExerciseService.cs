using GuitarTrainer.Components;
using GuitarTrainer.Dtos;
using GuitarTrainer.Enums;

namespace GuitarTrainer.Services
{
    public class BendExerciseService
    {
        public async Task<BendExerciseResultDto> ProccessBendRecordingAsync(Record record, BendType bendType) 
        {
            var centDifference = await CalculateBendDifferenceInCentsAsync(record);
            var bendCents = (double)bendType;
            var result = CreateBendExerciseResult(centDifference, bendCents);
            return result;
        }
        private BendExerciseResultDto CreateBendExerciseResult(double differenceInCents, double bendCents) 
        {
            double resultDifference = bendCents - differenceInCents;
            //Almost not noticable difference in pitch
            if (resultDifference <= 5.0 && resultDifference >= -5)
            {
                return new BendExerciseResultDto(true, resultDifference);
            }
            //Noticable for a trained ear
            else if (resultDifference <= 10 && resultDifference >= -10)
            {
                return new BendExerciseResultDto(true, resultDifference);
            }
            //Okay result 
            else if (resultDifference <= 15 && resultDifference >= -15)
            {
                return new BendExerciseResultDto(true, resultDifference);
            }
            //Difference to the point, where the note is noticably placed between the semitones. Not Acceptable :) 
            else
            {
                return new BendExerciseResultDto(false, resultDifference);
            }
        }
        private async Task<double> CalculateBendDifferenceInCentsAsync(Record record)
        {
            var points = await record.GetCentsValuesOverTimeAsync();
            var ordered = points
                .Select(p => p.RelativeCents)
                .OrderBy(x => x)
                .ToList();
            var lower = ordered.Take(ordered.Count / 10);
            var upper = ordered.TakeLast(ordered.Count / 10);

            double start = lower.Average();
            double end = upper.Average();
            return end - start;
        }
    }
}
