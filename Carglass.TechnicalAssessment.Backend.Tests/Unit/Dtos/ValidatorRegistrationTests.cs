using Autofac;
using Carglass.TechnicalAssessment.Backend.Dtos;
using FluentValidation;

namespace Carglass.TechnicalAssessment.Backend.Tests.Unit.Dtos;

public class ValidatorRegistrationTests
{
    private readonly IContainer _container;

    public ValidatorRegistrationTests()
    {
        var builder = new ContainerBuilder();
        builder.RegisterModule<Carglass.TechnicalAssessment.Backend.Dtos.Module>();
        _container = builder.Build();
    }

    public static IEnumerable<object[]> DtoTypes() =>
        typeof(IDto).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IDto).IsAssignableFrom(t))
            .Select(t => new object[] { t });

    [Theory]
    [MemberData(nameof(DtoTypes))]
    public void Every_dto_has_a_registered_validator(Type dtoType)
    {
        var validatorType = typeof(IValidator<>).MakeGenericType(dtoType);

        Assert.True(_container.IsRegistered(validatorType),
            $"No hay validador registrado para {dtoType.Name}.");
    }
}
