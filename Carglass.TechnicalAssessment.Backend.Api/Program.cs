using Autofac;
using Autofac.Extensions.DependencyInjection;
using Carglass.TechnicalAssessment.Backend.Api.Middlewares;
using Carglass.TechnicalAssessment.Backend.Api.Serialization;
using Carglass.TechnicalAssessment.Backend.Dtos;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Carglass.TechnicalAssessment.Backend.Api.Authentication;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services
    .AddEndpointsApiExplorer()
        .AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Carglass Technical Assessment API",
                Version = "v1",
                Description = "CRUD de clientes y productos."
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));

            var dtosXml = $"{typeof(ClientDto).Assembly.GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, dtosXml));

            options.MapType<DateTime>(() => new OpenApiSchema
            {
                Type = "string",
                Example = new OpenApiString("2019-01-09 14:26:17")
            });

            options.AddSecurityDefinition(ApiKeyAuthenticationHandler.SchemeName, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Name = "X-Api-Key",
                Description = "API key. En Development: dev-api-key-carglass"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = ApiKeyAuthenticationHandler.SchemeName }
                    },
                    Array.Empty<string>()
                }
            });
        })
    .AddAutoMapper(typeof(Carglass.TechnicalAssessment.Backend.BL.Module).Assembly);

    // Authentication
    builder.Services
       .AddAuthentication(ApiKeyAuthenticationHandler.SchemeName)
       .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(
           ApiKeyAuthenticationHandler.SchemeName,
           options => builder.Configuration.GetSection("Authentication").Bind(options));

    builder.Services.AddAuthorization(options =>
    {
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
    });

// Use and configure Autofac
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
    containerBuilder
        .RegisterModule<Carglass.TechnicalAssessment.Backend.BL.Module>()
        .RegisterModule<Carglass.TechnicalAssessment.Backend.DL.Module>()
        .RegisterModule<Carglass.TechnicalAssessment.Backend.Dtos.Module>();
});

var app = builder.Build();

// Request Logging Middleware
app.UseMiddleware<RequestLoggingMiddleware>();
// Exception Middleware
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger()
       .UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Validate API Key for security reasons, avoiding running the application with empty or missing API Key
var apiKey = builder.Configuration["Authentication:ApiKey"];
if (string.IsNullOrWhiteSpace(apiKey))
    throw new InvalidOperationException("Falta la configuración 'Authentication:ApiKey'.");

app.Run();

public partial class Program { }