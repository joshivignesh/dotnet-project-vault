using KedaWorker.Worker.Queue;

namespace KedaWorker.Tests;

public class InMemoryQueueTests
{
    [Fact]
    public void Enqueue_IncreasesCount()
    {
        var queue = new InMemoryQueue();
        queue.Enqueue(MakeOrder());
        Assert.Equal(1, queue.Count);
        Assert.Equal(1, queue.TotalEnqueued);
    }

    [Fact]
    public void Dequeue_DecreasesCountAndReturnsTrueWhenHasItem()
    {
        var queue = new InMemoryQueue();
        var order = MakeOrder();
        queue.Enqueue(order);

        var result = queue.TryDequeue(out var dequeued);

        Assert.True(result);
        Assert.NotNull(dequeued);
        Assert.Equal(order.OrderId, dequeued!.OrderId);
        Assert.Equal(0, queue.Count);
        Assert.Equal(1, queue.TotalProcessed);
    }

    [Fact]
    public void Dequeue_ReturnsFalseWhenEmpty()
    {
        var queue = new InMemoryQueue();
        var result = queue.TryDequeue(out var dequeued);
        Assert.False(result);
        Assert.Null(dequeued);
    }

    [Fact]
    public void MultipleEnqueue_PreservesOrder()
    {
        var queue = new InMemoryQueue();
        var ids = Enumerable.Range(0, 5).Select(_ => Guid.NewGuid()).ToList();
        foreach (var id in ids)
            queue.Enqueue(MakeOrder(id));

        foreach (var expectedId in ids)
        {
            queue.TryDequeue(out var msg);
            Assert.Equal(expectedId, msg!.OrderId);
        }
    }

    [Fact]
    public void TotalEnqueued_TracksAllEnqueued()
    {
        var queue = new InMemoryQueue();
        for (int i = 0; i < 10; i++)
            queue.Enqueue(MakeOrder());

        // Dequeue half
        for (int i = 0; i < 5; i++)
            queue.TryDequeue(out _);

        Assert.Equal(10, queue.TotalEnqueued);
        Assert.Equal(5, queue.TotalProcessed);
        Assert.Equal(5, queue.Count);
    }

    private static OrderMessage MakeOrder(Guid? id = null) => new(
        OrderId: id ?? Guid.NewGuid(),
        CustomerId: "CUST-001",
        ProductSku: "SKU-LAPTOP",
        Quantity: 1,
        TotalAmount: 999.99m,
        EnqueuedAt: DateTime.UtcNow
    );
}
