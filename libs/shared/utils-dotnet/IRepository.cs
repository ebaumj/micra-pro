namespace MicraPro.Shared.UtilsDotnet;

public interface IRepository<T> : IReadOnlyRepository<T>
    where T : IEntity
{
    Task AddAsync(T entity, CancellationToken ct);
    Task SaveAsync(CancellationToken ct);
    Task DeleteAsync(Guid entityId, CancellationToken ct);
}
