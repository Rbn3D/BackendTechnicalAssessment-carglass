using Microsoft.AspNetCore.Hosting;

namespace Carglass.TechnicalAssessment.Backend.Tests.Component;

public class ApiFactory : WebApplicationFactory<Program>
{
    public const string ApiKey = "test-api-key";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("Authentication:ApiKey", ApiKey);
    }

    public HttpClient CreateAuthenticatedClient()
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add("X-Api-Key", ApiKey);
        return client;
    }
}
