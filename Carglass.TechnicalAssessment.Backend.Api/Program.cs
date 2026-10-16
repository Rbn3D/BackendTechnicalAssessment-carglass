using Autofac;
using Autofac.Extensions.DependencyInjection;
using Carglass.TechnicalAssessment.Backend.Api.Middlewares;
using Carglass.TechnicalAssessment.Backend.Api.Serialization;
using Carglass.TechnicalAssessment.Backend.Dtos;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter()));

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
        })
    .AddAutoMapper(typeof(Carglass.TechnicalAssessment.Backend.BL.Module).Assembly);

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

app.UseHttpsRedirection()
   .UseAuthorization();

app.MapControllers();
app.Run();

public partial class Program { }