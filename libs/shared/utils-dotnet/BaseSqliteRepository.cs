using Microsoft.EntityFrameworkCore;

namespace MicraPro.Shared.UtilsDotnet;

public abstract class BaseSqliteRepository<T> : IRepository<T>
    where T : class, IEntity
{
    public async Task<T> GetByIdAsync(Guid id, CancellationToken ct) =>
        await (await GetEntitiesAsync(ct)).SingleAsync(e => e.Id == id, ct);

    public async Task<IReadOnlyCollection<T>> GetAllAsync(CancellationToken ct) =>
        await (await GetEntitiesAsync(ct)).ToArrayAsync(ct);

    public async Task AddAsync(T entity, CancellationToken ct) =>
        await (await GetEntitiesAsync(ct)).AddAsync(entity, ct);

    public async Task SaveAsync(CancellationToken ct) =>
        await (await GetContextAsync(ct)).SaveChangesAsync(ct);

    public async Task DeleteAsync(Guid entityId, CancellationToken ct) =>
        await (await GetEntitiesAsync(ct)).Where(e => e.Id == entityId).ExecuteDeleteAsync(ct);

    protected abstract Task<DbSet<T>> GetEntitiesAsync(CancellationToken ct);
    protected abstract Task<DbContext> GetContextAsync(CancellationToken ct);
}
