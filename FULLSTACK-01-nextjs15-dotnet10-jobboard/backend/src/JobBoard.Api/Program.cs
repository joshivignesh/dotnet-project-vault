using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JobBoard.Api.Data;
using JobBoard.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\mssqllocaldb;Database=JobBoardDb;Trusted_Connection=True;MultipleActiveResultSets=true";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

var jwtSection = builder.Configuration.GetSection("Jwt");
var signingKey = jwtSection["Key"] ?? "dev-only-change-me-please-change-me";
var issuer = jwtSection["Issuer"] ?? "JobBoardApi";
var audience = jwtSection["Audience"] ?? "JobBoardWeb";
var keyBytes = Encoding.UTF8.GetBytes(signingKey);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
        };
    });

builder.Services.AddAuthorization();

var frontendOrigin = builder.Configuration["Frontend:AllowedOrigin"] ?? "http://localhost:3000";
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
        policy.WithOrigins(frontendOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("frontend");
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();

    if (!await db.Jobs.AnyAsync())
    {
        db.Jobs.AddRange(
            new JobPosting
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Title = "Senior .NET Engineer",
                Company = "Northstar Labs",
                Location = "Bengaluru, India",
                WorkMode = "Hybrid",
                PostedOn = DateOnly.FromDateTime(DateTime.UtcNow.Date),
                CreatedBy = "seed"
            },
            new JobPosting
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Title = "Platform Backend Engineer",
                Company = "Orbit Systems",
                Location = "Remote",
                WorkMode = "Remote",
                PostedOn = DateOnly.FromDateTime(DateTime.UtcNow.Date),
                CreatedBy = "seed"
            });

        await db.SaveChangesAsync();
    }
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
    .WithName("Health");

app.MapGet("/api/public", () => Results.Ok(new { message = "Public endpoint is reachable." }))
    .WithName("PublicEndpoint");

app.MapPost("/api/auth/token", (LoginRequest request) =>
{
    if (request.Username != "demo" || request.Password != "demo")
    {
        return Results.Unauthorized();
    }

    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, request.Username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Name, request.Username),
        new Claim(ClaimTypes.Role, "Recruiter")
    };

    var credentials = new SigningCredentials(
        new SymmetricSecurityKey(keyBytes),
        SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer,
        audience,
        claims,
        expires: DateTime.UtcNow.AddMinutes(30),
        signingCredentials: credentials);

    var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
    return Results.Ok(new { accessToken, tokenType = "Bearer", expiresInMinutes = 30 });
})
    .WithName("IssueToken");

app.MapGet("/api/jobs", async (AppDbContext db) =>
    Results.Ok(await db.Jobs
        .OrderByDescending(job => job.PostedOn)
        .ToListAsync()))
    .RequireAuthorization()
    .WithName("GetJobs");

app.MapGet("/api/jobs/{id:guid}", async (Guid id, AppDbContext db) =>
{
    var job = await db.Jobs.FirstOrDefaultAsync(x => x.Id == id);
    return job is null ? Results.NotFound() : Results.Ok(job);
})
    .RequireAuthorization()
    .WithName("GetJobById");

app.MapPost("/api/jobs", async (CreateJobRequest request, ClaimsPrincipal user, AppDbContext db) =>
{
    var job = new JobPosting
    {
        Id = Guid.NewGuid(),
        Title = request.Title,
        Company = request.Company,
        Location = request.Location,
        WorkMode = request.WorkMode,
        PostedOn = DateOnly.FromDateTime(DateTime.UtcNow.Date),
        CreatedBy = user.Identity?.Name ?? "unknown"
    };

    db.Jobs.Add(job);
    await db.SaveChangesAsync();
    return Results.Created($"/api/jobs/{job.Id}", job);
})
    .RequireAuthorization()
    .WithName("CreateJob");

app.Run();

public record CreateJobRequest(
    string Title,
    string Company,
    string Location,
    string WorkMode);

public record LoginRequest(string Username, string Password);

public partial class Program;
