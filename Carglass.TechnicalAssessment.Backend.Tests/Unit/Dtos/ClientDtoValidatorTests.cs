using Carglass.TechnicalAssessment.Backend.Dtos;
using FluentValidation.TestHelper;

namespace Carglass.TechnicalAssessment.Backend.Tests.Unit.Dtos;

public class ClientDtoValidatorTests
{
    private readonly ClientDtoValidator _validator = new();

    private static ClientDto ValidDto() => new()
    {
        Id = 1,
        DocType = "nif",
        DocNum = "12345678Z",
        Email = "test@sample.com",
        GivenName = "Test",
        FamilyName1 = "User",
        Phone = "600000000"
    };

    [Fact]
    public void Valid_dto_has_no_errors()
    {
        var result = _validator.TestValidate(ValidDto());

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData("12345678Z")]
    [InlineData("11223344E")]   // el de la cliente semilla
    [InlineData("12345678z")]   // minúscula: aceptada por [A-Za-z]
    public void Nif_with_valid_format_has_no_docnum_error(string docNum)
    {
        var dto = ValidDto();
        dto.DocNum = docNum;

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.DocNum);
    }

    [Theory]
    [InlineData("1234567Z")]      // 7 dígitos
    [InlineData("123456789")]     // 9 dígitos, sin letra
    [InlineData("12345678")]      // sin letra
    [InlineData("ABCDEFGHZ")]     // letras en vez de dígitos
    [InlineData("12345678ZZ")]    // sobra una letra/ancla final
    [InlineData("X12345678Z")]    // prefijo/ancla inicial
    [InlineData("1234 5678Z")]    // espacio interior
    [InlineData("١٢٣٤٥٦٧٨Z")]     // dígitos no ASCII
    [InlineData("12345678Z\n")]   // salto de línea final
    public void Nif_with_invalid_format_has_docnum_error(string docNum)
    {
        var dto = ValidDto();
        dto.DocNum = docNum;

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.DocNum);
    }

    [Theory]
    [InlineData("nif")]
    [InlineData("NIF")]
    [InlineData("Nif")]
    public void Nif_rule_applies_regardless_of_doctype_case(string docType)
    {
        var dto = ValidDto();
        dto.DocType = docType;
        dto.DocNum = "1234567Z";

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.DocNum)
              .WithErrorMessage("El NIF debe tener 8 dígitos seguidos de 1 letra.");
    }

    [Theory]
    [InlineData("nie", "X1234567L")]
    [InlineData("cif", "B12345678")]
    [InlineData("pasaporte", "AB123456")]
    public void Non_nif_doctype_skips_nif_format(string docType, string docNum)
    {
        var dto = ValidDto();
        dto.DocType = docType;
        dto.DocNum = docNum;

        var result = _validator.TestValidate(dto);

        result.ShouldNotHaveValidationErrorFor(x => x.DocNum);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Empty_doctype_reports_doctype_error_only(string? docType)
    {
        var dto = ValidDto();
        dto.DocType = docType!;
        dto.DocNum = "no-es-un-nif";

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.DocType);
        result.ShouldNotHaveValidationErrorFor(x => x.DocNum);
    }

    [Fact]
    public void Empty_docnum_reports_required_error()
    {
        var dto = ValidDto();
        dto.DocNum = "";

        var result = _validator.TestValidate(dto);

        result.ShouldHaveValidationErrorFor(x => x.DocNum)
              .WithErrorMessage("El número de documento es necesario.");
    }

    [Fact]
    public void Id_zero_has_id_error()
    {
        var dto = ValidDto();
        dto.Id = 0;

        _validator.TestValidate(dto)
            .ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("El Id del cliente es necesario.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("no-es-un-email")]
    public void Invalid_email_has_email_error(string email)
    {
        var dto = ValidDto();
        dto.Email = email;

        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Fact]
    public void Docnum_longer_than_12_has_docnum_error()
    {
        var dto = ValidDto();
        dto.DocType = "pasaporte";          // evita que salte también la regla NIF
        dto.DocNum = "ABCDEFGHIJKLM";       // 13 caracteres

        _validator.TestValidate(dto).ShouldHaveValidationErrorFor(x => x.DocNum);
    }
}
