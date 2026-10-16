using System.Net;

namespace Carglass.TechnicalAssessment.Backend.Tests.Component;

public class HealthCheckEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HealthCheckEndpointsTests(WebApplicationFactory<Program> factory)
        => _client = factory.CreateClient();

    [Theory]
    [InlineData("/HealthCheck/KeepAlive")]
    [InlineData("/healthcheck/keepalive")]
    public async Task KeepAlive_returns_200(string url)
    {
        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task KeepAlive_is_not_cacheable()
    {
        var response = await _client.GetAsync("/HealthCheck/KeepAlive");

        Assert.True(response.Headers.CacheControl?.NoStore);
    }

    [Fact]
    public async Task KeepAlive_only_accepts_get()
    {
        var response = await _client.PostAsync("/HealthCheck/KeepAlive", null);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode); // Because the request is not authenticated, it returns 401 instead of 405
    }
}
