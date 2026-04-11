using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace MinimalApiJwtOpenApi.Tests;

public sealed class AuthAndSecureEndpointTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Health_ReturnsOk()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task IssueToken_WithValidCredentials_ReturnsBearerToken()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/token", new
        {
            Username = "demo",
            Password = "demo"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var payload = await response.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.NotNull(payload);
        Assert.False(string.IsNullOrWhiteSpace(payload.AccessToken));
        Assert.Equal("Bearer", payload.TokenType);
    }

    [Fact]
    public async Task SecureEndpoint_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/secure");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task SecureEndpoint_WithValidToken_ReturnsOk()
    {
        var token = await GetAccessTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/secure");
        response.EnsureSuccessStatusCode();

        var payload = await response.Content.ReadFromJsonAsync<SecureResponse>();
        Assert.NotNull(payload);
        Assert.Equal("demo", payload.User);
    }

    private async Task<string> GetAccessTokenAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/token", new
        {
            Username = "demo",
            Password = "demo"
        });

        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<TokenResponse>();
        Assert.NotNull(payload);
        return payload.AccessToken;
    }

    private sealed record TokenResponse(string AccessToken, string TokenType, int ExpiresInMinutes);

    private sealed record SecureResponse(string Message, string User);
}
