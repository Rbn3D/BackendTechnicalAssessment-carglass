using Carglass.TechnicalAssessment.Backend.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Json;

namespace Carglass.TechnicalAssessment.Backend.Tests.Component;

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

    [Fact]
    public async Task GetById_nonexistent_returns_404()
    {
        var response = await _client.GetAsync("/clients/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);

        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal(404, problem!.Status);
    }

    [Fact]
    public async Task Create_returns_201_with_location_header()
    {
        var response = await _client.PostAsJsonAsync("/clients", NewClient(2006, "55555555K"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/clients/2006", response.Headers.Location?.AbsolutePath);

        var follow = await _client.GetAsync(response.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, follow.StatusCode);
    }

    [Fact]
    public async Task Create_duplicated_id_returns_409()
    {
        var dto = NewClient(id: 1, docNum: "33333333P");

        var response = await _client.PostAsJsonAsync("/clients", dto);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        var stored = await _client.GetFromJsonAsync<ClientDto>("/clients/1");
        Assert.Equal("11223344E", stored!.DocNum); // Must return the original client, not the new one
    }

    [Fact]
    public async Task Create_duplicated_docnum_returns_409()
    {
        var dto = NewClient(id: 200, docNum: "11223344E");

        var response = await _client.PostAsJsonAsync("/clients", dto);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        var check = await _client.GetAsync("/clients/200");
        Assert.Equal(HttpStatusCode.NotFound, check.StatusCode); // Must return 404 (not created due to conflict)
    }

    // Nifs invalidos
    [Theory]
    [InlineData("1234567Z")]    // 7 dígitos
    [InlineData("123456789")]   // sin letra
    [InlineData("ABCDEFGHZ")]   // letras en vez de dígitos
    [InlineData("12345678ZZ")]  // sobra una letra
    public async Task Create_invalid_nif_returns_400(string docNum)
    {
        var dto = NewClient(id: 300, docNum: docNum);

        var response = await _client.PostAsJsonAsync("/clients", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Contains("DocNum", problem!.Errors.Keys);
    }

    [Fact]
    public async Task Update_nonexistent_returns_404()
    {
        var response = await _client.PutAsJsonAsync("/clients", NewClient(999999, "11111111H"));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_nonexistent_returns_404()
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, "/clients")
        {
            Content = JsonContent.Create(NewClient(999999, "11111111H"))
        };

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_then_GetById_returns_updated()
    {
        var original = NewClient(id: 400, docNum: "11111112H");
        var response = await _client.PostAsJsonAsync("/clients", original);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var updated = NewClient(id: 400, docNum: "11111112H");
        updated.Email = "updated@sample.com";
        updated.Phone = "699999999";

        response = await _client.PutAsJsonAsync("/clients", updated);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var stored = await _client.GetFromJsonAsync<ClientDto>("/clients/400");

        // Verify that the stored client has the updated values
        Assert.Equal("updated@sample.com", stored!.Email);
        Assert.Equal("699999999", stored.Phone);
        Assert.Equal("11111112H", stored.DocNum);
    }

    [Fact]
    public async Task Delete_then_GetById_returns_404()
    {
        var dto = NewClient(id: 500, docNum: "22222222J");
        (await _client.PostAsJsonAsync("/clients", dto)).EnsureSuccessStatusCode();

        var request = new HttpRequestMessage(HttpMethod.Delete, "/clients")
        {
            Content = JsonContent.Create(dto)
        };
        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        var check = await _client.GetAsync("/clients/500");
        Assert.Equal(HttpStatusCode.NotFound, check.StatusCode);
    }

    private static ClientDto NewClient(int id, string docNum) => new()
    {
        Id = id,
        DocType = "nif",
        DocNum = docNum,
        Email = $"client{id}@sample.com",
        GivenName = "Test",
        FamilyName1 = "User",
        Phone = "600000000"
    };
}