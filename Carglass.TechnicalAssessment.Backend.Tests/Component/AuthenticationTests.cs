using System.Net;

namespace Carglass.TechnicalAssessment.Backend.Tests.Component;

public class AuthenticationTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public AuthenticationTests(ApiFactory factory) => _factory = factory;

    [Fact]
    public async Task Request_without_api_key_returns_401()
    {
        var response = await _factory.CreateClient().GetAsync("/clients");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("wrong-key")]
    [InlineData("")]
    [InlineData("test-api-key ")]   // espacio final
    public async Task Request_with_invalid_api_key_returns_401(string apiKey)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.TryAddWithoutValidation("X-Api-Key", apiKey);

        var response = await client.GetAsync("/clients");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Request_with_valid_api_key_returns_200()
    {
        var response = await _factory.CreateAuthenticatedClient().GetAsync("/clients");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task KeepAlive_does_not_require_api_key()
    {
        var response = await _factory.CreateClient().GetAsync("/HealthCheck/KeepAlive");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Swagger_does_not_require_api_key()
    {
        var response = await _factory.CreateClient().GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
