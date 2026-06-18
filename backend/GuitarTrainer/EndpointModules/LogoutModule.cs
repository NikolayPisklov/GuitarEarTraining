using Carter;
using GuitarTrainer.Model;
using Microsoft.AspNetCore.Identity;

namespace GuitarTrainer.EndpointModules
{
    public class LogoutModule : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/logout", async (SignInManager<AppUser> signInManager) =>
            {
                await signInManager.SignOutAsync();
                return Results.Ok();
            });
        }
    }
}
