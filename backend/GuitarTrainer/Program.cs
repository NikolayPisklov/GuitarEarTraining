using GuitarTrainer.Dtos;
using GuitarTrainer.Enums;
using GuitarTrainer.Model;
using GuitarTrainer.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NAudio.Wave;
using NWaves.Audio;
using NWaves.FeatureExtractors;
using NWaves.FeatureExtractors.Options;
using NWaves.Signals;
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
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    .UseSnakeCaseNamingConvention();
});

builder.Services
    .AddIdentityApiEndpoints<AppUser>()
    .AddEntityFrameworkStores<AppDbContext>();
builder.Services.AddAuthorization();

builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<PitchExerciseService>();
builder.Services.AddScoped<IExerciseResultService, ExerciseResultService>();
builder.Services.AddScoped<BendExerciseService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) 
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityApi<AppUser>();

using var scoped = app.Services.CreateScope();

app.MapPost("/logout", async (SignInManager<AppUser> signInManager) =>
{
    await signInManager.SignOutAsync();
    return Results.Ok();
});

var profile = app.MapGroup("/profile");
profile.RequireAuthorization();

profile.MapGet("/isUserWithName", async (ClaimsPrincipal user, IUserProfileService profileService) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
    var userGuidId = UserIdParsingService.ParseUserId(userId);
    bool result = await profileService.IsUserWithNameAsync(userGuidId);
    return Results.Ok(result);
});

profile.MapGet("/getUserName", async (ClaimsPrincipal user, IUserProfileService profileService) => 
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
    var userGuidId = UserIdParsingService.ParseUserId(userId);
    var result = await profileService.GetUserFullNameAsync(userGuidId);
    return Results.Ok(result);
});

profile.MapPost("/updateUserName", async (ClaimsPrincipal user, IUserProfileService profileService,
    UserNameDto dto) =>
{ 
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
    var userGuidId = UserIdParsingService.ParseUserId(userId);
    await profileService.UpdateUserFullNameAsync(userGuidId, dto.FirstName, dto.LastName);
    return Results.Ok();
});

var pitch = app.MapGroup("/pitch");
pitch.RequireAuthorization();

pitch.MapGet("/getSamples", async (PitchExerciseService pitchService) => 
{
    var samples = await pitchService.GetSamplesForExerciseAsync();
    return Results.Ok(samples);
});
pitch.MapGet("/getAnswerOptions", async (PitchExerciseService pitchService) => 
{
    var options = await pitchService.GetAnswerOptionsAsync();
    return Results.Ok(options);
});

var sampleExerciseResult = app.MapGroup("/exerciseResult");
sampleExerciseResult.RequireAuthorization();

sampleExerciseResult.MapPost("/addAttempt", async (IExerciseResultService resultService, 
    ClaimsPrincipal user, AddRequestAttemptDto dto) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
    var userGuidId = UserIdParsingService.ParseUserId(userId);
    await resultService.InsertAttemptAsync(dto.Answers, dto.ExerciseId, userGuidId);
    return Results.Ok();
});
sampleExerciseResult.MapGet("/getLatestAttemptScore", async (IExerciseResultService resultService, int exerciseId,
    ClaimsPrincipal user) => 
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
    double score = await resultService.GetScoreOfLastAttemptAsync(exerciseId, 
        UserIdParsingService.ParseUserId(userId));
    return Results.Ok(score);
});

var bendsExerciseGroup = app.MapGroup("/bends");
bendsExerciseGroup.RequireAuthorization();

bendsExerciseGroup.MapPost("/whole-step", async (IFormFile file, BendExerciseService service) => 
{
    var result = await service.ProcessUserBendFileAsync(file, BendType.WholeStepCents);
    return Results.Ok(result);
}).DisableAntiforgery();
bendsExerciseGroup.MapPost("/half-step", async (IFormFile file, BendExerciseService service) => 
{
}).DisableAntiforgery();

app.Run();
