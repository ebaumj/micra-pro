using MicraPro.BeanManagement.Domain.StorageAccess;
using MicraPro.Shared.UtilsDotnet;
using Microsoft.EntityFrameworkCore;

namespace MicraPro.BeanManagement.Infrastructure.StorageAccess;

internal class KeyValueStore(MigratedContextProvider<BeanManagementDbContext> contextProvider)
    : IKeyValueStore
{
    private async Task<DbSet<KeyValueEntry>> GetEntitiesAsync(CancellationToken ct) =>
        (await contextProvider.GetContextAsync(ct)).KeyValueEntries;

    private async Task<DbContext> GetContextAsync(CancellationToken ct) =>
        await contextProvider.GetContextAsync(ct);

    public async Task<string> GetAsync(string key, CancellationToken ct) =>
        (await (await GetEntitiesAsync(ct)).SingleAsync(e => e.Key == key, ct)).JsonValue;

    public async Task<string?> TryGetAsync(string key, CancellationToken ct)
    {
        try
        {
            return await GetAsync(key, ct);
        }
        catch
        {
            return null;
        }
    }

    public async Task AddOrUpdateAsync(string key, string jsonValue, CancellationToken ct)
    {
        var entities = await GetEntitiesAsync(ct);
        var entity = await entities.SingleOrDefaultAsync(e => e.Key == key, ct);
        if (entity is not null)
            entity.JsonValue = jsonValue;
        else
            await entities.AddAsync(new KeyValueEntry(key, jsonValue), ct);
        await SaveAsync(ct);
    }

    public async Task DeleteAsync(string key, CancellationToken ct)
    {
        await (await GetEntitiesAsync(ct)).Where(e => e.Key == key).ExecuteDeleteAsync(ct);
        await SaveAsync(ct);
    }

    private async Task SaveAsync(CancellationToken ct) =>
        await (await GetContextAsync(ct)).SaveChangesAsync(ct);
}
