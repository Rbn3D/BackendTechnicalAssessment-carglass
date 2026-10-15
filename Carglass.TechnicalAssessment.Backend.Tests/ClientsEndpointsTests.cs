using Carglass.TechnicalAssessment.Backend.Dtos;
using System.Net.Http.Json;

namespace Carglass.TechnicalAssessment.Backend.Tests;

public class ClientsEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ClientsEndpointTests(WebApplicationFactory<Program> factory)
        => _client = factory.CreateClient();

    [Fact]
    public async Task Create_then_GetById_returns_created_client()
    {
        var dto = new ClientDto { Id = 100, DocType = "nif", DocNum = "12345678Z", Email = "test@sample.com", GivenName = "Test", FamilyName1 = "User", Phone = "123456789" };

        var post = await _client.PostAsJsonAsync("/clients", dto);
        post.EnsureSuccessStatusCode();

        var result = await _client.GetFromJsonAsync<ClientDto>("/clients/100");
        Assert.Equal(dto.DocNum, result!.DocNum);
    }
}