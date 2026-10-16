using Carglass.TechnicalAssessment.Backend.Entities;

namespace Carglass.TechnicalAssessment.Backend.DL.Repositories;

public class ProductIMRepository : InMemoryRepository<Product>
{
    public ProductIMRepository() : base(Seed()) { }

    private static IEnumerable<Product> Seed() => new[]
    {
        new Product
        {
            Id = 1111111,
            ProductName = "Cristal ventanilla delantera",
            ProductType = 25,
            NumTerminal = 933933933,
            SoldAt = new DateTime(2019, 1, 9, 14, 26, 17)
        }
    };
}
