using System.Collections.Concurrent;

namespace KedaWorker.Worker.Queue;

/// <summary>
/// In-process queue that simulates an Azure Service Bus queue for local development.
/// In production, swap this with the real Azure.Messaging.ServiceBus client.
/// </summary>
public sealed class InMemoryQueue
{
    private readonly ConcurrentQueue<OrderMessage> _queue = new();
    private int _enqueued;
    private int _processed;

    public int Count => _queue.Count;
    public int TotalEnqueued => _enqueued;
    public int TotalProcessed => _processed;

    public void Enqueue(OrderMessage message)
    {
        _queue.Enqueue(message);
        Interlocked.Increment(ref _enqueued);
    }

    public bool TryDequeue(out OrderMessage? message)
    {
        if (_queue.TryDequeue(out message))
        {
            Interlocked.Increment(ref _processed);
            return true;
        }
        message = null;
        return false;
    }
}

public record OrderMessage(
    Guid OrderId,
    string CustomerId,
    string ProductSku,
    int Quantity,
    decimal TotalAmount,
    DateTime EnqueuedAt
);
