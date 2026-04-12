using CleanArchitecture.Application;
using CleanArchitecture.Application.Products.Commands;
using CleanArchitecture.Application.Products.Queries;
using CleanArchitecture.Infrastructure;
using CleanArchitecture.Infrastructure.Data;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Seed demo data on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await AppDbContext.SeedAsync(db);
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // Scalar UI: GET /scalar
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/openapi/v1.json", "CleanArchitecture API v1"));
}

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
   .WithName("Health").WithTags("Health");

// Products endpoints
var products = app.MapGroup("/api/products").WithTags("Products");

products.MapGet("/", async (IMediator mediator, string? category, CancellationToken ct) =>
{
    var result = await mediator.Send(new GetAllProductsQuery(category), ct);
    return Results.Ok(result);
}).WithName("GetAllProducts").WithSummary("List all products, optionally filtered by category");

products.MapGet("/{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) =>
{
    var result = await mediator.Send(new GetProductByIdQuery(id), ct);
    return result is null ? Results.NotFound() : Results.Ok(result);
}).WithName("GetProductById").WithSummary("Get a product by ID");

products.MapPost("/", async (CreateProductCommand command, IMediator mediator, CancellationToken ct) =>
{
    var result = await mediator.Send(command, ct);
    return Results.Created($"/api/products/{result.Id}", result);
}).WithName("CreateProduct").WithSummary("Create a new product");

products.MapPut("/{id:guid}", async (Guid id, UpdateProductCommand command, IMediator mediator, CancellationToken ct) =>
{
    var updated = command with { Id = id };
    var success = await mediator.Send(updated, ct);
    return success ? Results.NoContent() : Results.NotFound();
}).WithName("UpdateProduct").WithSummary("Update an existing product");

products.MapDelete("/{id:guid}", async (Guid id, IMediator mediator, CancellationToken ct) =>
{
    var success = await mediator.Send(new DeleteProductCommand(id), ct);
    return success ? Results.NoContent() : Results.NotFound();
}).WithName("DeleteProduct").WithSummary("Delete a product");

app.Run();
