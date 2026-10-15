using FluentValidation;
using System.Text.RegularExpressions;

namespace Carglass.TechnicalAssessment.Backend.Dtos;

public class ClientDtoValidator : AbstractValidator<ClientDto>
{
    private const string NifDocType = "nif";
    private static readonly Regex NifRegex = new(@"^[0-9]{8}[A-Za-z]\z", RegexOptions.Compiled); // Regex compiled for performance

    public ClientDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(default(int))
            .WithMessage("El Id del cliente es necesario.");

        RuleFor(x => x.DocType)
            .NotEmpty()
            .WithMessage("El tipo de documento es necesario.")
            .MaximumLength(25)
            .WithMessage("El tipo de documento tiene una longitud máxima de 25 caracteres.");

        RuleFor(x => x.DocNum)
            .NotEmpty()
            .WithMessage("El número de documento es necesario.")
            .MaximumLength(12)
            .WithMessage("El número de documento tiene una longitud máxima de 12 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El email es necesario.")
            .EmailAddress()
            .WithMessage("El email no cumple el formato adecuado.");

        RuleFor(x => x.DocNum)
            .Matches(NifRegex)
            .WithMessage("El NIF debe tener 8 dígitos seguidos de 1 letra.")
            .When(x => string.Equals(x.DocType, NifDocType, StringComparison.OrdinalIgnoreCase));
    }
}
