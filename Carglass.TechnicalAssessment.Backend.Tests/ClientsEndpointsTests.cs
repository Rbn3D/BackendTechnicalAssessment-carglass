using Carglass.TechnicalAssessment.Backend.Dtos;
using System.Net;
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

    [Fact]
    public async Task GetAll_returns_seed_client()
    {
        var response = await _client.GetAsync("/clients");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var clients = await response.Content.ReadFromJsonAsync<List<ClientDto>>();
        Assert.NotNull(clients);
        Assert.Contains(clients!, c => c.Id == 1 && c.DocNum == "11223344E");
    }

    [Fact]
    public async Task GetById_existing_returns_client()
    {
        var response = await _client.GetAsync("/clients/1");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var client = await response.Content.ReadFromJsonAsync<ClientDto>();
        Assert.NotNull(client);
        Assert.Equal(1, client!.Id);
        Assert.Equal("nif", client.DocType);
        Assert.Equal("11223344E", client.DocNum);
        Assert.Equal("eromani@sample.com", client.Email);
        Assert.Equal("Enriqueta", client.GivenName);
        Assert.Equal("Romani", client.FamilyName1);
        Assert.Equal("668668668", client.Phone);
    }
}