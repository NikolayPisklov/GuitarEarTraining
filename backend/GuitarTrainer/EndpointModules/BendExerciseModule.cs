using Carter;
using GuitarTrainer.Enums;
using GuitarTrainer.Services;

namespace GuitarTrainer.EndpointModules
{
    public class BendExerciseModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/bends")
                .RequireAuthorization();

            group.MapPost("/whole-step", async (IFormFile file, BendExerciseService service) =>
            {
                var result = await service.ProcessUserBendFileAsync(file, BendType.WholeStepCents);
                return Results.Ok(result);
            });
            group.MapPost("/half-step", async (IFormFile file, BendExerciseService service) =>
            {
            }).DisableAntiforgery();
        }
    }
}
