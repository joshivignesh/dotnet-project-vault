using EfCoreVectorSearch.Domain;
using EfCoreVectorSearch.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();

    if (!await db.Documents.AnyAsync())
    {
        db.Documents.AddRange(
            new Document
            {
                Id = Guid.NewGuid(),
                Title = "Senior .NET Platform Engineer",
                Content = "Build distributed services with .NET 10, APIs, and SQL.",
                Embedding = [0.91f, 0.13f, 0.76f]
            },
            new Document
            {
                Id = Guid.NewGuid(),
                Title = "AI Search Engineer",
                Content = "Implement semantic retrieval and ranking with vector similarity.",
                Embedding = [0.82f, 0.69f, 0.15f]
            });

        await db.SaveChangesAsync();
    }
}

app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
    .WithName("Health");

app.MapGet("/api/documents", async (AppDbContext db) =>
{
    var documents = await db.Documents
        .OrderByDescending(x => x.CreatedUtc)
        .Select(x => new { x.Id, x.Title, x.Content, x.CreatedUtc })
        .ToListAsync();

    return Results.Ok(documents);
})
    .WithName("GetDocuments");

app.MapPost("/api/search", async (SearchRequest request, AppDbContext db) =>
{
    var documents = await db.Documents.ToListAsync();

    var ranked = documents
        .Select(document => new
        {
            document.Id,
            document.Title,
            document.Content,
            Score = CosineSimilarity(request.QueryVector, document.Embedding)
        })
        .OrderByDescending(x => x.Score)
        .Take(5)
        .ToList();

    return Results.Ok(ranked);
})
    .WithName("VectorSearch");

app.Run();

static double CosineSimilarity(float[] left, float[] right)
{
    if (left.Length == 0 || right.Length == 0 || left.Length != right.Length)
    {
        return 0;
    }

    double dot = 0;
    double leftMagnitude = 0;
    double rightMagnitude = 0;

    for (var i = 0; i < left.Length; i++)
    {
        dot += left[i] * right[i];
        leftMagnitude += left[i] * left[i];
        rightMagnitude += right[i] * right[i];
    }

    if (leftMagnitude == 0 || rightMagnitude == 0)
    {
        return 0;
    }

    return dot / (Math.Sqrt(leftMagnitude) * Math.Sqrt(rightMagnitude));
}

public record SearchRequest(float[] QueryVector);
