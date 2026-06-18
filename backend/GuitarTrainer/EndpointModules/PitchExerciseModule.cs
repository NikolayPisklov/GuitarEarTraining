using Carter;
using GuitarTrainer.Services;

namespace GuitarTrainer.EndpointModules
{
    public class PitchExerciseModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/pitch")
                .RequireAuthorization();

            group.MapGet("/get-samples", async (PitchExerciseService pitchService) =>
            {
                var samples = await pitchService.GetSamplesForExerciseAsync();
                return Results.Ok(samples);
            });
            group.MapGet("/get-answer-options", async (PitchExerciseService pitchService) =>
            {
                var options = await pitchService.GetAnswerOptionsAsync();
                return Results.Ok(options);
            });
        }
    }
}
