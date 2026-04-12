using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace RateLimitingMiddleware.Tests;

// Each test class gets its own factory instance so rate limiter state is isolated.

public class HealthTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory = new();
    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task Health_NeverRateLimited_AlwaysReturns200()
    {
        var client = _factory.CreateClient();
        for (var i = 0; i < 15; i++)
        {
            var response = await client.GetAsync("/health");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    [Fact]
    public async Task Health_ReturnsJsonContentType()
    {
        var client = _factory.CreateClient();
        var resp = await client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        Assert.StartsWith("application/json", resp.Content.Headers.ContentType?.MediaType ?? "");
    }
}

public class FixedWindowTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory = new();
    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task FixedWindow_Allows5ThenRejects()
    {
        var client = _factory.CreateClient();
        var successes = 0;
        var rejections = 0;

        for (var i = 0; i < 8; i++)
        {
            var resp = await client.GetAsync("/api/public/data");
            if (resp.StatusCode == HttpStatusCode.OK) successes++;
            else if (resp.StatusCode == HttpStatusCode.TooManyRequests) rejections++;
        }

        Assert.Equal(5, successes);
        Assert.True(rejections >= 1, "Expected at least one 429 after limit exceeded");
    }

    [Fact]
    public async Task FixedWindow_RejectedResponse_HasJsonErrorBody()
    {
        var client = _factory.CreateClient();
        HttpResponseMessage? lastResponse = null;

        for (var i = 0; i < 8; i++)
            lastResponse = await client.GetAsync("/api/public/data");

        Assert.NotNull(lastResponse);
        Assert.Equal(HttpStatusCode.TooManyRequests, lastResponse.StatusCode);

        var body = await lastResponse.Content.ReadAsStringAsync();
        Assert.Contains("error", body);
        Assert.Contains("Too many requests", body);
    }
}

public class SlidingWindowTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory = new();
    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task SlidingWindow_Allows20Requests()
    {
        var client = _factory.CreateClient();
        var successes = 0;

        for (var i = 0; i < 20; i++)
        {
            var resp = await client.GetAsync("/api/reports/summary");
            if (resp.StatusCode == HttpStatusCode.OK) successes++;
        }

        Assert.Equal(20, successes);
    }
}

public class TokenBucketTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory = new();
    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task TokenBucket_Allows10BurstThenRejects()
    {
        var client = _factory.CreateClient();
        var successes = 0;
        var rejections = 0;

        for (var i = 0; i < 15; i++)
        {
            var resp = await client.GetAsync("/api/search?q=test");
            if (resp.StatusCode == HttpStatusCode.OK) successes++;
            else if (resp.StatusCode == HttpStatusCode.TooManyRequests) rejections++;
        }

        Assert.Equal(10, successes);
        Assert.True(rejections >= 1);
    }
}

public class ConcurrencyTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory = new();
    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task Concurrency_SingleRequest_Returns200()
    {
        var client = _factory.CreateClient();
        var response = await client.PostAsync("/api/compute", null);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
