// ============================================================
// FILE: Program.cs
// PURPOSE: This is where your app STARTS. Think of it as the
//   front door of your house -- everything enters here.
//   We configure: services, middleware, telemetry, and routing.
// ============================================================

// STEP 1: Configure Serilog BEFORE anything else
// WHY? If the app crashes during startup, we still get logs
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore",
        Serilog.Events.LogEventLevel.Warning)  // Suppress noisy framework logs
    .Enrich.FromLogContext()        // Allows adding custom properties per-request
    .Enrich.WithProperty("Application", "OrderApi")
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] " +
        "{Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.Seq("http://localhost:5341")  // Seq = searchable log dashboard
    .CreateLogger();

try
{
    Log.Information("Starting OrderApi application");

    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();  // Replace default .NET logging with Serilog

    // STEP 2: Register our custom services (Dependency Injection)
    // ANALOGY: Think of DI as a restaurant kitchen. You tell the
    //   kitchen (DI container) what ingredients (services) you need,
    //   and it prepares them for you when a customer (request) arrives.
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // STEP 3: Configure OpenTelemetry (the observability backbone)
    // This tells .NET to automatically track every HTTP request,
    // every database query, and every HTTP call to other services.
    builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource => resource
            .AddService(
                serviceName: "OrderApi",
                serviceVersion: "1.0.0"))
        .WithTracing(tracing => tracing
            .AddAspNetCoreInstrumentation()   // Auto-trace HTTP requests
            .AddHttpClientInstrumentation()    // Auto-trace outgoing HTTP calls
            .AddOtlpExporter(opts =>           // Send traces to collector
                opts.Endpoint = new Uri("http://localhost:4317")))
        .WithMetrics(metrics => metrics
            .AddAspNetCoreInstrumentation()   // Auto-count requests, latency
            .AddRuntimeInstrumentation()       // Track CPU, memory, GC
            .AddMeter("OrderApi.Business")  // Our custom metrics
            .AddPrometheusExporter());         // Expose /metrics endpoint

    var app = builder.Build();

    // STEP 4: Middleware pipeline (ORDER MATTERS!)
    // ANALOGY: Middleware is like airport security checkpoints.
    //   Every request passes through each checkpoint in order.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging();  // Log every HTTP request automatically
    app.MapControllers();
    app.MapPrometheusScrapingEndpoint();  // Expose GET /metrics for Prometheus

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();  // Ensure all logs are written before exit
}
