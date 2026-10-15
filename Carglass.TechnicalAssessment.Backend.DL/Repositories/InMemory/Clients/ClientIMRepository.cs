using Carglass.TechnicalAssessment.Backend.Entities;

namespace Carglass.TechnicalAssessment.Backend.DL.Repositories;

public class ClientIMRepository : InMemoryRepository<Client>
{
    public ClientIMRepository() : base(Seed()) { }

    private static IEnumerable<Client> Seed() => new[]
    {
        new Client
        {
            Id = 1,
            DocType = "nif",
            DocNum = "11223344E",
            Email = "eromani@sample.com",
            GivenName = "Enriqueta",
            FamilyName1 = "Romani",
            Phone = "668668668"
        }
    };
}