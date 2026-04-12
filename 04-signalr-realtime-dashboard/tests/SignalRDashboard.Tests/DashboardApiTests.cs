using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;

namespace SignalRDashboard.Tests;

public class DashboardApiTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task HealthEndpoint_ReturnsOk()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/health");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("healthy", body);
    }

    [Fact]
    public async Task SignalRHubNegotiate_ReturnsOk()
    {
        var client = factory.CreateClient();
        var response = await client.PostAsync("/hubs/dashboard/negotiate?negotiateVersion=1", null);
        // 200 OK means SignalR negotiate succeeded
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task StaticDashboard_ReturnsHtml()
    {
        var client = factory.CreateClient();
        var response = await client.GetAsync("/index.html");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("SignalR", content);
    }
}
