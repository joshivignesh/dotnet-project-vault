using EfCoreVectorSearch.Domain;
using EfCoreVectorSearch.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

// Seed knowledge-base documents with embeddings on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.EnsureCreatedAsync();

    if (!await db.Documents.AnyAsync())
    {
        var seed = new[]
        {
            new Document { Id = Guid.NewGuid(), Title = "Clean Architecture in .NET 10", Category = "Architecture",
                Content = "Clean Architecture separates concerns into domain, application, infrastructure, and API layers using dotnet csharp." },
            new Document { Id = Guid.NewGuid(), Title = "CQRS with MediatR", Category = "Architecture",
                Content = "CQRS csharp architecture patterns use commands and queries via MediatR for clean separation in dotnet API applications." },
            new Document { Id = Guid.NewGuid(), Title = "EF Core 10 Performance Tips", Category = "Database",
                Content = "Improve database sql performance with EF Core using compiled queries, split queries, and proper index strategy." },
            new Document { Id = Guid.NewGuid(), Title = "JWT Authentication in ASP.NET Core", Category = "Security",
                Content = "Implement authentication security with JWT bearer tokens in dotnet web api applications using middleware." },
            new Document { Id = Guid.NewGuid(), Title = "KEDA AutoScaling on Kubernetes", Category = "Cloud",
                Content = "Kubernetes kubernetes cloud autoscaling using KEDA with Azure Service Bus triggers for distributed messaging worker services." },
            new Document { Id = Guid.NewGuid(), Title = "Rate Limiting Strategies", Category = "API",
                Content = "Protect API web endpoints with fixed-window and sliding-window rate limiting policies in ASP.NET Core dotnet." },
            new Document { Id = Guid.NewGuid(), Title = "SignalR Real-Time Dashboards", Category = "Realtime",
                Content = "Build realtime web signalr dashboards with ASP.NET Core hubs and JavaScript clients for live metric updates." },
            new Document { Id = Guid.NewGuid(), Title = "Azure Container Apps Deployment", Category = "Cloud",
                Content = "Deploy cloud docker containers to Azure cloud using Container Apps with automatic azure kubernetes scaling." },
            new Document { Id = Guid.NewGuid(), Title = "Docker Multi-Stage Builds for .NET", Category = "DevOps",
                Content = "Optimize docker container images using multi-stage builds for dotnet csharp applications with minimal final image size." },
            new Document { Id = Guid.NewGuid(), Title = "Vector Search with EF Core", Category = "AI",
                Content = "Implement AI vector semantic search in dotnet using EF Core with cosine similarity for document retrieval and machine learning." },
        };

        // Generate embeddings for all seed documents
        foreach (var doc in seed)
            doc.Embedding = EmbeddingService.Generate($"{doc.Title} {doc.Content}");

        db.Documents.AddRange(seed);
        await db.SaveChangesAsync();
    }
}

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("Health").WithTags("Health");

app.MapGet("/api/documents", async (AppDbContext db, string? category) =>
{
    var query = db.Documents.AsQueryable();
    if (!string.IsNullOrWhiteSpace(category))
        query = query.Where(d => d.Category == category);

    var docs = await query
        .OrderByDescending(x => x.CreatedUtc)
        .Select(x => new { x.Id, x.Title, x.Content, x.Category, x.CreatedUtc })
        .ToListAsync();

    return Results.Ok(docs);
}).WithName("GetDocuments").WithTags("Documents")
  .WithSummary("List all documents, optionally filtered by category");

app.MapPost("/api/search", async (TextSearchRequest request, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(request.Query))
        return Results.BadRequest(new { error = "Query text is required" });

    // Generate embedding for query text
    var queryEmbedding = EmbeddingService.Generate(request.Query);

    // Load all documents and compute cosine similarity in-process
    // In production: use pgvector or Azure AI Search for server-side vector ops
    var documents = await db.Documents.ToListAsync();

    var topK = request.TopK is > 0 and <= 20 ? request.TopK.Value : 5;

    var results = documents
        .Select(d => new
        {
            d.Id,
            d.Title,
            d.Content,
            d.Category,
            Score = EmbeddingService.CosineSimilarity(queryEmbedding, d.Embedding)
        })
        .OrderByDescending(x => x.Score)
        .Take(topK)
        .Where(x => x.Score > 0.01f)
        .Select(x => new
        {
            x.Id,
            x.Title,
            x.Content,
            x.Category,
            Similarity = Math.Round(x.Score, 4)
        })
        .ToList();

    return Results.Ok(new
    {
        query = request.Query,
        count = results.Count,
        results
    });
}).WithName("SemanticSearch").WithTags("Search")
  .WithSummary("Semantic vector search over knowledge-base documents");

app.MapPost("/api/documents", async (CreateDocumentRequest request, AppDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Content))
        return Results.BadRequest(new { error = "Title and Content are required" });

    var doc = new Document
    {
        Id = Guid.NewGuid(),
        Title = request.Title,
        Content = request.Content,
        Category = request.Category ?? "General",
        Embedding = EmbeddingService.Generate($"{request.Title} {request.Content}"),
        CreatedUtc = DateTime.UtcNow
    };

    db.Documents.Add(doc);
    await db.SaveChangesAsync();

    return Results.Created($"/api/documents/{doc.Id}", new { doc.Id, doc.Title, doc.Category, doc.CreatedUtc });
}).WithName("CreateDocument").WithTags("Documents")
  .WithSummary("Add a new document and auto-generate its embedding vector");

app.Run();

record TextSearchRequest(string Query, int? TopK = 5);
record CreateDocumentRequest(string Title, string Content, string? Category);
public record SearchRequest(float[] QueryVector);
