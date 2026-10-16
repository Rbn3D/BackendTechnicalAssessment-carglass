namespace Carglass.TechnicalAssessment.Backend.Dtos;

public class ProductDto : IDto
{
    public int Id { get; set; }
    public string ProductName { get; set; }
    public int ProductType { get; set; }
    public int NumTerminal { get; set; }
    /// <summary>
    /// Fecha y hora en que se completó la venta. Formato: "yyyy-MM-dd HH:mm:ss"
    /// </summary>
    public DateTime SoldAt { get; set; }
}
