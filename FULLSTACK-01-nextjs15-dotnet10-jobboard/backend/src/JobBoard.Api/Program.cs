var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var jobs = new List<JobPosting>
{
    new(Guid.Parse("11111111-1111-1111-1111-111111111111"), "Senior .NET Engineer", "Northstar Labs", "Bengaluru, India", "Hybrid", DateOnly.FromDateTime(DateTime.UtcNow.Date)),
    new(Guid.Parse("22222222-2222-2222-2222-222222222222"), "Platform Backend Engineer", "Orbit Systems", "Remote", "Remote", DateOnly.FromDateTime(DateTime.UtcNow.Date))
};

app.MapGet("/health", () => Results.Ok(new { status = "ok" }))
    .WithName("Health");

app.MapGet("/api/jobs", () => Results.Ok(jobs))
    .WithName("GetJobs");

app.MapGet("/api/jobs/{id:guid}", (Guid id) =>
{
    var job = jobs.FirstOrDefault(x => x.Id == id);
    return job is null ? Results.NotFound() : Results.Ok(job);
})
    .WithName("GetJobById");

app.MapPost("/api/jobs", (CreateJobRequest request) =>
{
    var job = new JobPosting(
        Guid.NewGuid(),
        request.Title,
        request.Company,
        request.Location,
        request.WorkMode,
        DateOnly.FromDateTime(DateTime.UtcNow.Date));

    jobs.Add(job);
    return Results.Created($"/api/jobs/{job.Id}", job);
})
    .WithName("CreateJob");

app.Run();

public record JobPosting(
    Guid Id,
    string Title,
    string Company,
    string Location,
    string WorkMode,
    DateOnly PostedOn);

public record CreateJobRequest(
    string Title,
    string Company,
    string Location,
    string WorkMode);
