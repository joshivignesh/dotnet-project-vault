using SonarGithubActions.Api.Models;
using SonarGithubActions.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSingleton<OrderCalculator>();
builder.Services.AddSingleton<TextAnalyzer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", utc = DateTime.UtcNow }))
   .WithName("Health").WithTags("Health");

// ── Order pricing endpoints ────────────────────────────────────────────────
var orders = app.MapGroup("/api/orders").WithTags("Orders");

orders.MapPost("/calculate", (OrderRequest request, OrderCalculator calculator) =>
{
    if (request.Items is null || request.Items.Count == 0)
        return Results.BadRequest(new { error = "Order must contain at least one item." });

    var result = calculator.Calculate(request);
    return Results.Ok(result);
})
.WithName("CalculateOrder")
.WithSummary("Calculate order total with tax and tier discounts");

orders.MapGet("/discount-tiers", () => Results.Ok(OrderCalculator.DiscountTiers))
.WithName("GetDiscountTiers")
.WithSummary("Returns current discount tier thresholds");

// ── Text analysis endpoints ────────────────────────────────────────────────
var text = app.MapGroup("/api/text").WithTags("Text Analysis");

text.MapPost("/analyze", (TextRequest request, TextAnalyzer analyzer) =>
{
    if (string.IsNullOrWhiteSpace(request.Content))
        return Results.BadRequest(new { error = "Content must not be empty." });

    var result = analyzer.Analyze(request.Content);
    return Results.Ok(result);
})
.WithName("AnalyzeText")
.WithSummary("Returns word count, sentence count, and readability metrics");

app.Run();

// Expose for WebApplicationFactory
public partial class Program { }
