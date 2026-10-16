using FluentValidation;

namespace Carglass.TechnicalAssessment.Backend.Dtos;

public class ProductDtoValidator : AbstractValidator<ProductDto>
{
    public ProductDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(default(int))
            .WithMessage("El Id del producto es necesario.");

        RuleFor(x => x.ProductName)
            .NotEmpty()
            .WithMessage("El nombre del producto es necesario.");
    }
}
