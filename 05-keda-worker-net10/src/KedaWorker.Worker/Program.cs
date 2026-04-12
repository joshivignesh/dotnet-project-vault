using KedaWorker.Worker;
using KedaWorker.Worker.Queue;
using KedaWorker.Worker.Services;

var builder = Host.CreateApplicationBuilder(args);

// Singleton queue shared between producer and worker
builder.Services.AddSingleton<InMemoryQueue>();

// Producer simulates incoming orders; in production replace with ServiceBusClient
builder.Services.AddHostedService<OrderProducerSimulator>();

// Worker consumes from the queue
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
