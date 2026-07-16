using Microsoft.AspNetCore.Antiforgery;

namespace GuitarTrainer.EndpointFilters
{
    public class AntiforgeryFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(
            EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var antiforgery = context.HttpContext.RequestServices
            .GetRequiredService<IAntiforgery>();

            await antiforgery.ValidateRequestAsync(context.HttpContext);

            return await next(context);
        }
    }
}
