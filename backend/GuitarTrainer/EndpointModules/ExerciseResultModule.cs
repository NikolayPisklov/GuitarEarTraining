using Carter;
using GuitarTrainer.Dtos;
using GuitarTrainer.Services;
using System.Security.Claims;

namespace GuitarTrainer.EndpointModules
{
    public class ExerciseResultModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/exercise-result")
                .RequireAuthorization();

            group.MapPost("/add-attempt", async (IExerciseResultService resultService, ClaimsPrincipal user, AddRequestAttemptDto dto) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
                await resultService
                    .InsertAttemptAsync(dto.Answers, dto.ExerciseId, UserIdParsingService.ParseUserId(userId));
                return Results.Ok();
            });
            group.MapGet("/get-latest-attempt-score", async (IExerciseResultService resultService, int exerciseId, ClaimsPrincipal user) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
                double score = await resultService
                    .GetScoreOfLastAttemptAsync(exerciseId, UserIdParsingService.ParseUserId(userId));
                return Results.Ok(score);
            });

        }
    }
}
