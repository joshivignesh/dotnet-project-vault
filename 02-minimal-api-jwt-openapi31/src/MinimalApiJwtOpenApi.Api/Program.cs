using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var jwtSection = builder.Configuration.GetSection("Jwt");
var signingKey = jwtSection["Key"] ?? "dev-only-change-me-please-change-me";
var issuer = jwtSection["Issuer"] ?? "MinimalApiJwtOpenApi";
var audience = jwtSection["Audience"] ?? "MinimalApiJwtOpenApiClient";
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
    .WithName("Health");

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
        new Claim(ClaimTypes.Role, "User")
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

app.MapGet("/api/public", () => Results.Ok(new { message = "Public endpoint is reachable." }))
    .WithName("PublicEndpoint");

app.MapGet("/api/secure", (ClaimsPrincipal user) =>
        Results.Ok(new
        {
            message = "Secure endpoint is reachable with a valid bearer token.",
            user = user.Identity?.Name ?? "unknown"
        }))
    .RequireAuthorization()
    .WithName("SecureEndpoint");

app.Run();

public record LoginRequest(string Username, string Password);
