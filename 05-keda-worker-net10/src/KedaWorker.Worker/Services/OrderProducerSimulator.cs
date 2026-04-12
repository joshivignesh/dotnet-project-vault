using KedaWorker.Worker.Queue;

namespace KedaWorker.Worker.Services;

/// <summary>
/// Simulates order messages arriving on the queue at random intervals.
/// Mimics the producer side — in production this would be your order service publishing to Azure Service Bus.
/// </summary>
public sealed class OrderProducerSimulator(
    InMemoryQueue queue,
    ILogger<OrderProducerSimulator> logger) : BackgroundService
{
    private readonly Random _rng = new();
    private static readonly string[] Customers = ["CUST-001", "CUST-042", "CUST-117", "CUST-209", "CUST-355"];
    private static readonly string[] Skus = ["SKU-LAPTOP", "SKU-MONITOR", "SKU-KEYBOARD", "SKU-HEADSET", "SKU-WEBCAM"];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Order producer started — will emit orders to simulate queue load");

        while (!stoppingToken.IsCancellationRequested)
        {
            // Burst: enqueue 1-5 orders at a time
            var batch = _rng.Next(1, 6);
            for (int i = 0; i < batch; i++)
            {
                var msg = new OrderMessage(
                    OrderId: Guid.NewGuid(),
                    CustomerId: Customers[_rng.Next(Customers.Length)],
                    ProductSku: Skus[_rng.Next(Skus.Length)],
                    Quantity: _rng.Next(1, 10),
                    TotalAmount: Math.Round((decimal)(_rng.NextDouble() * 2000 + 50), 2),
                    EnqueuedAt: DateTime.UtcNow
                );
                queue.Enqueue(msg);
            }

            logger.LogDebug("Produced {Batch} orders. Queue depth: {Depth}", batch, queue.Count);

            // Pause 2–5 seconds between bursts
            await Task.Delay(TimeSpan.FromSeconds(_rng.Next(2, 6)), stoppingToken);
        }
    }
}
