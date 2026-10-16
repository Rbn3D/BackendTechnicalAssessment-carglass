using Carglass.TechnicalAssessment.Backend.Dtos;
using FluentValidation.TestHelper;

namespace Carglass.TechnicalAssessment.Backend.Tests.Unit.Dtos;

public class ProductDtoValidatorTests
{
    private readonly ProductDtoValidator _validator = new();

    private static ProductDto ValidDto() => new()
    {
        Id = 1,
        ProductName = "Cristal ventanilla delantera",
        ProductType = 25,
        NumTerminal = 933933933,
        SoldAt = new DateTime(2019, 1, 9, 14, 26, 17)
    };

    [Fact]
    public void Valid_dto_has_no_errors()
    {
        _validator.TestValidate(ValidDto()).ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Id_zero_has_id_error()
    {
        var dto = ValidDto();
        dto.Id = 0;

        _validator.TestValidate(dto)
            .ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("El Id del producto es necesario.");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Empty_product_name_has_product_name_error(string? productName)
    {
        var dto = ValidDto();
        dto.ProductName = productName!;

        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.ProductName);
    }

    [Fact]
    public void Only_id_and_product_name_are_validated()
    {
        var dto = new ProductDto { Id = 1, ProductName = "X" };   // resto con valores por defecto

        _validator.TestValidate(dto).ShouldNotHaveAnyValidationErrors();
    }
}
