using Carglass.TechnicalAssessment.Backend.Entities;

namespace Carglass.TechnicalAssessment.Backend.DL.Repositories;

public class ClientIMRepository : ICrudRepository<Client>
{
    private ICollection<Client> _clients;

    public ClientIMRepository()
    {
        _clients = new HashSet<Client>()
        {
            new Client()
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

    public IEnumerable<Client> GetAll()
    {
        return _clients;
    }

    public Client? GetById(params object[] keyValues)
    {
        return _clients.SingleOrDefault(x => x.Id.Equals(keyValues[0]));
    }

    public void Create(Client item)
    {
        _clients.Add(item);
    }

    public void Update(Client item)
    {
        var toUpdateItem = _clients.Single(x => x.Id.Equals(item.Id));

        UpdateClientFields(item, toUpdateItem);
    }

    public void Delete(Client item)
    {
        var toDeleteItem = _clients.Single(x => x.Id.Equals(item.Id));

        _clients.Remove(toDeleteItem);
    }

    private static void UpdateClientFields(Client item, Client toUpdateItem)
    {
        toUpdateItem.DocType = item.DocType;
        toUpdateItem.DocNum = item.DocNum;
        toUpdateItem.Email = item.Email;
        toUpdateItem.GivenName = item.GivenName;
        toUpdateItem.FamilyName1 = item.FamilyName1;
        toUpdateItem.Phone = item.Phone;
    }
}
