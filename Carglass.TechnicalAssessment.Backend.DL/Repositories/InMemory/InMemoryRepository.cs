using System.Collections.Concurrent;
using Carglass.TechnicalAssessment.Backend.Entities;

namespace Carglass.TechnicalAssessment.Backend.DL.Repositories;

public abstract class InMemoryRepository<TEntity> : ICrudRepository<TEntity>
    where TEntity : class, IEntity
{
    private readonly ConcurrentDictionary<int, TEntity> _items;

    protected InMemoryRepository(IEnumerable<TEntity> seed)
    {
        _items = new ConcurrentDictionary<int, TEntity>(seed.ToDictionary(e => e.Id));
    }

    public IEnumerable<TEntity> GetAll()
    {
        return _items.Values.OrderBy(e => e.Id).ToList();
    }

    public TEntity? GetById(params object[] keyValues)
    {
        if (keyValues.Length != 1 || keyValues[0] is not int id)
            throw new ArgumentException("Se esperaba un único Id de tipo int.", nameof(keyValues));

        return _items.TryGetValue(id, out var entity) ? entity : null;
    }

    public void Create(TEntity item)
    {
        if (!_items.TryAdd(item.Id, item))
            throw new InvalidOperationException($"Ya existe una entidad con Id '{item.Id}'.");
    }

    public void Update(TEntity item)
    {
        if (!_items.ContainsKey(item.Id))
            throw new KeyNotFoundException($"No existe ninguna entidad con Id '{item.Id}'.");

        _items[item.Id] = item;
    }

    public void Delete(TEntity item)
    {
        if (!_items.TryRemove(item.Id, out _))
            throw new KeyNotFoundException($"No existe ninguna entidad con Id '{item.Id}'.");
    }
}
