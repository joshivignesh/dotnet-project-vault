using KedaWorker.Worker.Queue;

namespace KedaWorker.Worker;

/// <summary>
/// KEDA-aware worker that processes order messages from the queue.
/// In production this polls Azure Service Bus; KEDA scales the pod count
/// based on queue depth using a ScaledObject manifest.
/// </summary>
public class Worker(
    InMemoryQueue queue,
    ILogger<Worker> logger) : BackgroundService
{
    private const int PollIntervalMs = 500;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Order processing worker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            if (queue.TryDequeue(out var message) && message is not null)
            {
                await ProcessOrderAsync(message, stoppingToken);
            }
            else
            {
                await Task.Delay(PollIntervalMs, stoppingToken);
            }
        }

        logger.LogInformation("Order processing worker stopped. Processed: {Count}", queue.TotalProcessed);
    }

    private async Task ProcessOrderAsync(OrderMessage order, CancellationToken ct)
    {
        var latency = DateTime.UtcNow - order.EnqueuedAt;

        logger.LogInformation(
            "Processing order {OrderId} | Customer: {CustomerId} | SKU: {Sku} | Qty: {Qty} | Total: {Total:C} | Queue latency: {Latency:F1}ms",
            order.OrderId, order.CustomerId, order.ProductSku, order.Quantity,
            order.TotalAmount, latency.TotalMilliseconds);

        // Simulate order processing work (validation, inventory, payment stub)
        await Task.Delay(Random.Shared.Next(50, 250), ct);

        logger.LogInformation("Order {OrderId} processed successfully", order.OrderId);
    }
}

