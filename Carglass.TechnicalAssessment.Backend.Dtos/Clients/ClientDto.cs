namespace Carglass.TechnicalAssessment.Backend.Dtos;

public class ClientDto : IDto
{
    public int Id { get; set; }
    /// <summary>
    /// Tipo de documento. Si es "nif", se valida el formato de NIF.
    /// </summary>
    public string DocType { get; set; }
    /// <summary>
    /// Identificador o número del documento asociado.
    /// </summary>
    /// <remarks>Si DocType es "nif", se valida el formato de NIF.</remarks>
    public string DocNum { get; set; }
    public string Email { get; set; }
    public string GivenName { get; set; }
    public string FamilyName1 { get; set; }
    public string Phone { get; set; }
}
