using Autofac;
using Autofac.Extensions.DependencyInjection;
using Carglass.TechnicalAssessment.Backend.Api.Middlewares;
using Carglass.TechnicalAssessment.Backend.Api.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new DateTimeJsonConverter()));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
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