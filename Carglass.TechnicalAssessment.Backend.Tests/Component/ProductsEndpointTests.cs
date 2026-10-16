using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Carglass.TechnicalAssessment.Backend.Tests.Component;

public class ProductsEndpointTests : IClassFixture<ApiFactory>
{
    public ProductsEndpointTests(ApiFactory factory)
         => _client = factory.CreateAuthenticatedClient();

    private const string SoldAt = "2019-01-09 14:26:17";

    private readonly HttpClient _client;

    private static object NewProduct(int id, string? productName = "Cristal ventanilla delantera") => new
    {
        id,
        productName,
        productType = 25,
        numTerminal = 933933933,
        soldAt = SoldAt
    };

    private async Task<JsonElement> GetProductJson(int id)
    {
        var response = await _client.GetAsync($"/products/{id}");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        return await response.Content.ReadFromJsonAsync<JsonElement>();
    }

    [Fact]
    public async Task GetById_seed_product_matches_contract()
    {
        var json = await GetProductJson(1111111);

        Assert.Equal(1111111, json.GetProperty("id").GetInt32());
        Assert.Equal("Cristal ventanilla delantera", json.GetProperty("productName").GetString());
        Assert.Equal(25, json.GetProperty("productType").GetInt32());
        Assert.Equal(933933933, json.GetProperty("numTerminal").GetInt32());
        Assert.Equal(SoldAt, json.GetProperty("soldAt").GetString());
    }

    [Fact]
    public async Task GetAll_returns_seed_product()
    {
        var response = await _client.GetAsync("/products");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var products = await response.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Contains(products.EnumerateArray(), p => p.GetProperty("id").GetInt32() == 1111111);
    }

    [Fact]
    public async Task Create_then_GetById_returns_created_product()
    {
        var response = await _client.PostAsJsonAsync("/products", NewProduct(2001));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var json = await GetProductJson(2001);
        Assert.Equal("Cristal ventanilla delantera", json.GetProperty("productName").GetString());
        Assert.Equal(SoldAt, json.GetProperty("soldAt").GetString());
    }

    [Fact]
    public async Task Create_returns_201_with_location_header()
    {
        var response = await _client.PostAsJsonAsync("/products", NewProduct(2006));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/products/2006", response.Headers.Location?.AbsolutePath);

        var follow = await _client.GetAsync(response.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, follow.StatusCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task Create_without_product_name_returns_400(string? productName)
    {
        var response = await _client.PostAsJsonAsync("/products", NewProduct(2002, productName));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Contains("ProductName", problem!.Errors.Keys);
    }

    [Theory]
    [InlineData("09/01/2019 14:26:17")]
    [InlineData("2019-13-01 14:26:17")] // mes 13
    [InlineData("no-es-una-fecha")]
    public async Task Create_with_invalid_sold_at_returns_400(string soldAt)
    {
        var product = new { id = 2005, productName = "Luna", productType = 25, numTerminal = 933933933, soldAt };

        var response = await _client.PostAsJsonAsync("/products", product);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Contains("$.soldAt", problem!.Errors.Keys);
    }

    [Fact]
    public async Task Create_with_id_zero_returns_400()
    {
        var response = await _client.PostAsJsonAsync("/products", NewProduct(0));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Contains("Id", problem!.Errors.Keys);
    }

    [Fact]
    public async Task Create_duplicated_id_returns_409()
    {
        var response = await _client.PostAsJsonAsync("/products", NewProduct(1111111));

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task GetById_nonexistent_returns_404()
    {
        var response = await _client.GetAsync("/products/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task Update_nonexistent_returns_404()
    {
        var response = await _client.PutAsJsonAsync("/products", NewProduct(999999));

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_nonexistent_returns_404()
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, "/products")
        {
            Content = JsonContent.Create(NewProduct(999999))
        };

        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_then_GetById_returns_updated()
    {
        (await _client.PostAsJsonAsync("/products", NewProduct(2003))).EnsureSuccessStatusCode();

        var response = await _client.PutAsJsonAsync("/products", NewProduct(2003, "Luna trasera"));

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        var json = await GetProductJson(2003);
        Assert.Equal("Luna trasera", json.GetProperty("productName").GetString());
    }

    [Fact]
    public async Task Delete_then_GetById_returns_404()
    {
        var product = NewProduct(2004);
        (await _client.PostAsJsonAsync("/products", product)).EnsureSuccessStatusCode();

        var request = new HttpRequestMessage(HttpMethod.Delete, "/products")
        {
            Content = JsonContent.Create(product)
        };
        var response = await _client.SendAsync(request);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync("/products/2004")).StatusCode);
    }
}
