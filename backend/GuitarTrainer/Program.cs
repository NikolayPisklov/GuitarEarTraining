using GuitarTrainer.Dtos;
using GuitarTrainer.Model;
using GuitarTrainer.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.Configure<CookieAuthenticationOptions>(
    IdentityConstants.ApplicationScheme,
    options =>
    {
        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    }
);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services
    .AddIdentityApiEndpoints<AppUser>()
    .AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddAuthorization();

builder.Services.AddScoped<IUserProfileService, UserProfileService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) 
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityApi<AppUser>();

using var scoped = app.Services.CreateScope();

//Endpoints
var authGroup = app.MapGroup("/api");
authGroup.RequireAuthorization();

authGroup.MapGet("/isUserWithName", async (ClaimsPrincipal user, IUserProfileService profileService) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
    Guid userGuidId;
    if(!Guid.TryParse(userId, out userGuidId))
    {
        return Results.Unauthorized();
    }
    bool result = await profileService.IsUserWithNameAsync(userGuidId);
    return Results.Ok(result);
});

authGroup.MapGet("/getUserName", async (ClaimsPrincipal user, IUserProfileService profileService) => 
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
    Guid userGuidId;
    if (!Guid.TryParse(userId, out userGuidId))
    {
        return Results.Unauthorized();
    }
    var result = await profileService.GetUserFullNameAsync(userGuidId);
    return Results.Ok(result);
});

authGroup.MapPost("/updateUserName", async (ClaimsPrincipal user, IUserProfileService profileService,
    UserNameDto dto) =>
{ 
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
    Guid userGuidId;
    if (!Guid.TryParse(userId, out userGuidId))
    {
        return Results.BadRequest("User Id parsing error!");
    }
    await profileService.UpdateUserFullNameAsync(userGuidId, dto.FirstName, dto.LastName);
    return Results.Ok();
});

app.Run();
