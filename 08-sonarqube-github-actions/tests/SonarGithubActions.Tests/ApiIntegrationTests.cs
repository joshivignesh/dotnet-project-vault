using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using SonarGithubActions.Api.Models;

namespace SonarGithubActions.Tests;

public class ApiIntegrationTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory = new();
    private readonly HttpClient _client;

    public ApiIntegrationTests() => _client = _factory.CreateClient();
    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task Health_Returns200WithHealthyStatus()
    {
        var resp = await _client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task OrderCalculate_ValidRequest_Returns200()
    {
        var request = new OrderRequest([new OrderItem("Widget", 50m, 3)]);
        var resp = await _client.PostAsJsonAsync("/api/orders/calculate", request);
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task OrderCalculate_EmptyItems_Returns400()
    {
        var resp = await _client.PostAsJsonAsync("/api/orders/calculate", new { items = Array.Empty<object>() });
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task DiscountTiers_Returns200WithList()
    {
        var resp = await _client.GetAsync("/api/orders/discount-tiers");
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task TextAnalyze_ValidContent_Returns200()
    {
        var request = new TextRequest("The quick brown fox jumps over the lazy dog.");
        var resp = await _client.PostAsJsonAsync("/api/text/analyze", request);
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
    }

    [Fact]
    public async Task TextAnalyze_EmptyContent_Returns400()
    {
        var resp = await _client.PostAsJsonAsync("/api/text/analyze", new { content = "" });
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }
}
