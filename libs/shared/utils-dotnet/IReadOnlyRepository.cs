namespace MicraPro.Shared.UtilsDotnet;

public interface IReadOnlyRepository<T>
    where T : IEntity
{
    Task<T> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyCollection<T>> GetAllAsync(CancellationToken ct);
}
