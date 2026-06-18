using Carter;
using GuitarTrainer.Dtos;
using GuitarTrainer.Services;
using System.Security.Claims;

namespace GuitarTrainer.EndpointModules
{
    public class ProfileModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            var group = app.MapGroup("/profile")
                .RequireAuthorization();

            group.MapGet("/is-user-with-name", async (ClaimsPrincipal user, IUserProfileService profileService) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var userGuidId = UserIdParsingService.ParseUserId(userId);
                bool result = await profileService.IsUserWithNameAsync(userGuidId);
                return Results.Ok(result);
            });

            group.MapGet("/get-user-name", async (ClaimsPrincipal user, IUserProfileService profileService) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var userGuidId = UserIdParsingService.ParseUserId(userId);
                var result = await profileService.GetUserFullNameAsync(userGuidId);
                return Results.Ok(result);
            });

            group.MapPost("/update-user-name", async (ClaimsPrincipal user, IUserProfileService profileService, UserNameDto dto) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var userGuidId = UserIdParsingService.ParseUserId(userId);
                await profileService.UpdateUserFullNameAsync(userGuidId, dto.FirstName, dto.LastName);
                return Results.Ok();
            });
        }
    }
}
