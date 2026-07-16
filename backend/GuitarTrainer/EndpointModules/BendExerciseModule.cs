using Carter;
using GuitarTrainer.Components;
using GuitarTrainer.EndpointFilters;
using GuitarTrainer.Enums;
using GuitarTrainer.Exceptions;
using GuitarTrainer.Services;
using System.Security.Claims;

namespace GuitarTrainer.EndpointModules
{
    public class BendExerciseModule : ICarterModule
    {
        private readonly long _maxRecordSize = 9 * 1024 * 1024;
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/bends")
                .RequireAuthorization()
                .AddEndpointFilter<AntiforgeryFilter>()
                ;

            group.MapPost("/whole-step", async (HttpRequest request, IExerciseResultService resultService,
                BendExerciseService service, ClaimsPrincipal user) =>
            {
                var sampleRate = ValidateBendRecord(request);
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var record = await request.GetRecordFromBody(sampleRate);
                var result = await service.ProccessBendRecordingAsync(record, BendType.WholeStepCents);
                await resultService
                    .InsertAttemptAsync(result, (int)Exercises.WholeStepBends, UserIdParsingService.ParseUserId(userId));
                return Results.Ok(result);
            });
            group.MapPost("/half-step", async (HttpRequest request, IExerciseResultService resultService,
                BendExerciseService service, ClaimsPrincipal user) =>
            {
                var sampleRate = ValidateBendRecord(request);
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var record = await request.GetRecordFromBody(sampleRate);
                var result = await service.ProccessBendRecordingAsync(record, BendType.HalfStepCents);
                await resultService
                    .InsertAttemptAsync(result, (int)Exercises.HalfStepBends, UserIdParsingService.ParseUserId(userId));
                return Results.Ok(result);
            });
        }
        private int ValidateBendRecord(HttpRequest request) 
        {
            if (!int.TryParse(request.Headers["X-Sample-Rate"], out int sampleRate))
            {
                throw new InvalidRecordException("Provided sample rate is invalid.");
            }
            if (request.ContentLength == 0)
            {
                throw new InvalidRecordException("Record is empty.");
            }
            if (request.ContentLength > _maxRecordSize)
            {
                throw new InvalidRecordException("Record is too long.");
            }
            return sampleRate;
        }
    }
}
