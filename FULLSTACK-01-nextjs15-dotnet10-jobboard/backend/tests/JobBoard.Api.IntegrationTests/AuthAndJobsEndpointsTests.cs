using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace JobBoard.Api.IntegrationTests;

public sealed class AuthAndJobsEndpointsTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

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
    public async Task GetJobs_WithoutToken_ReturnsUnauthorized()
    {
        var response = await _client.GetAsync("/api/jobs");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetJobs_WithValidToken_ReturnsSeededJobs()
    {
        var token = await GetAccessTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/jobs");
        response.EnsureSuccessStatusCode();

        var jobs = await response.Content.ReadFromJsonAsync<List<JobPostingDto>>();
        Assert.NotNull(jobs);
        Assert.True(jobs.Count >= 2);
    }

    [Fact]
    public async Task CreateJob_WithValidToken_CreatesJobForAuthenticatedRecruiter()
    {
        var token = await GetAccessTokenAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createRequest = new CreateJobRequest(
            Title: "Principal Platform Engineer",
            Company: "Acme Talent Cloud",
            Location: "Remote",
            WorkMode: "Remote");

        var createResponse = await _client.PostAsJsonAsync("/api/jobs", createRequest);
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdJob = await createResponse.Content.ReadFromJsonAsync<JobPostingDto>();
        Assert.NotNull(createdJob);
        Assert.Equal("demo", createdJob.CreatedBy);
        Assert.Equal(createRequest.Title, createdJob.Title);

        var getResponse = await _client.GetAsync($"/api/jobs/{createdJob.Id}");
        getResponse.EnsureSuccessStatusCode();

        var fetched = await getResponse.Content.ReadFromJsonAsync<JobPostingDto>();
        Assert.NotNull(fetched);
        Assert.Equal(createdJob.Id, fetched.Id);
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

    private sealed record CreateJobRequest(string Title, string Company, string Location, string WorkMode);

    private sealed record JobPostingDto(
        Guid Id,
        string Title,
        string Company,
        string Location,
        string WorkMode,
        DateOnly PostedOn,
        string CreatedBy);
}
