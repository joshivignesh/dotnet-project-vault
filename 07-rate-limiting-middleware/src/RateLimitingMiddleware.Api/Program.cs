using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Policy 1: Fixed window — 5 requests per 10s (tight public API throttle)
    options.AddFixedWindowLimiter("fixed-window", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromSeconds(10);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });

    // Policy 2: Sliding window — 20 requests per 30s, 3 segments
    options.AddSlidingWindowLimiter("sliding-window", opt =>
    {
        opt.PermitLimit = 20;
        opt.Window = TimeSpan.FromSeconds(30);
        opt.SegmentsPerWindow = 3;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 5;
    });

    // Policy 3: Token bucket — burst-friendly (10 tokens, replenish 2/sec)
    options.AddTokenBucketLimiter("token-bucket", opt =>
    {
        opt.TokenLimit = 10;
        opt.ReplenishmentPeriod = TimeSpan.FromSeconds(1);
        opt.TokensPerPeriod = 2;
        opt.AutoReplenishment = true;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });

    // Policy 4: Concurrency limiter — at most 3 concurrent expensive operations
    options.AddConcurrencyLimiter("concurrency", opt =>
    {
        opt.PermitLimit = 3;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;
    });

    // Global on-rejected callback with Retry-After header
    options.OnRejected = async (context, ct) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
            context.HttpContext.Response.Headers.RetryAfter =
                ((int)retryAfter.TotalSeconds).ToString();

        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            error = "Too many requests",
            retryAfterSeconds = context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var ra)
                ? (int?)ra.TotalSeconds : null
        }, ct);
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseRateLimiter();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .WithName("Health").WithTags("Health");

// Fixed-window: strict public API endpoint
app.MapGet("/api/public/data", () =>
    Results.Ok(new { policy = "fixed-window", message = "Public data response", utc = DateTime.UtcNow }))
   .RequireRateLimiting("fixed-window")
   .WithName("PublicData").WithTags("Fixed Window")
   .WithSummary("5 req / 10s — fixed window throttle");

// Sliding-window: dashboard/reporting endpoint
app.MapGet("/api/reports/summary", () =>
    Results.Ok(new { policy = "sliding-window", data = Enumerable.Range(1, 5).Select(i => new { id = i, value = i * 10 }) }))
   .RequireRateLimiting("sliding-window")
   .WithName("ReportsSummary").WithTags("Sliding Window")
   .WithSummary("20 req / 30s sliding window — suitable for dashboard queries");

// Token bucket: burst-tolerant search endpoint
app.MapGet("/api/search", (string? q) =>
    Results.Ok(new { policy = "token-bucket", query = q ?? "*", results = Array.Empty<string>() }))
   .RequireRateLimiting("token-bucket")
   .WithName("Search").WithTags("Token Bucket")
   .WithSummary("10 burst tokens, +2/sec — tolerates short bursts");

// Concurrency limiter: expensive background computation
app.MapPost("/api/compute", async (CancellationToken ct) =>
{
    await Task.Delay(200, ct); // simulate work
    return Results.Ok(new { policy = "concurrency", result = Guid.NewGuid() });
})
   .RequireRateLimiting("concurrency")
   .WithName("Compute").WithTags("Concurrency")
   .WithSummary("Max 3 concurrent executions with queue of 2");

app.Run();

// Expose for WebApplicationFactory
public partial class Program { }
