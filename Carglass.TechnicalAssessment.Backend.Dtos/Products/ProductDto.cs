namespace Carglass.TechnicalAssessment.Backend.Dtos;

public class ProductDto : IDto
{
    public int Id { get; set; }
    public string ProductName { get; set; }
    public int ProductType { get; set; }
    public int NumTerminal { get; set; }
    public DateTime SoldAt { get; set; }
}
