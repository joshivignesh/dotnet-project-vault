using SignalRDashboard.Api.Hubs;
using SignalRDashboard.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSignalR();
builder.Services.AddHostedService<MetricsSimulatorService>();

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(
                builder.Configuration.GetSection("Cors:Origins").Get<string[]>()
                    ?? ["http://localhost:3000", "http://localhost:5173"])
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()));

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseCors();
app.UseStaticFiles();

app.MapHub<DashboardHub>("/hubs/dashboard");

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .WithName("Health").WithTags("Health");

app.MapFallbackToFile("index.html");

app.Run();

// Expose for WebApplicationFactory in integration tests
public partial class Program { }
