using MicraPro.Shared.UtilsDotnet;
using Microsoft.EntityFrameworkCore;

namespace MicraPro.Shared.Infrastructure.KeyValueStore;

internal class KeyValueStoreBase(MigratedContextProvider<SharedDbContext> contextProvider)
{
    private async Task<DbSet<KeyValueEntry>> GetEntitiesAsync(CancellationToken ct) =>
        (await contextProvider.GetContextAsync(ct)).KeyValueStore;

    private async Task<DbContext> GetContextAsync(CancellationToken ct) =>
        await contextProvider.GetContextAsync(ct);

    private async Task<string> GetAsync(string key, CancellationToken ct) =>
        (await (await GetEntitiesAsync(ct)).SingleAsync(e => e.Key == key, ct)).JsonValue;

    private string ConnectKey(string key, string storeNamespace) => $"{storeNamespace}.{key}";

    private async Task SaveAsync(CancellationToken ct) =>
        await (await GetContextAsync(ct)).SaveChangesAsync(ct);

    public async Task<string?> TryGetAsync(string storeNamespace, string key, CancellationToken ct)
    {
        try
        {
            return await GetAsync(ConnectKey(key, storeNamespace), ct);
        }
        catch
        {
            return null;
        }
    }

    public async Task AddOrUpdateAsync(
        string storeNamespace,
        string key,
        string jsonValue,
        CancellationToken ct
    )
    {
        var entities = await GetEntitiesAsync(ct);
        var entity = await entities.SingleOrDefaultAsync(
            e => e.Key == ConnectKey(key, storeNamespace),
            ct
        );
        if (entity is not null)
            entity.JsonValue = jsonValue;
        else
            await entities.AddAsync(
                new KeyValueEntry(ConnectKey(key, storeNamespace), jsonValue),
                ct
            );
        await SaveAsync(ct);
    }

    public async Task DeleteAsync(string storeNamespace, string key, CancellationToken ct)
    {
        await (await GetEntitiesAsync(ct))
            .Where(e => e.Key == ConnectKey(key, storeNamespace))
            .ExecuteDeleteAsync(ct);
        await SaveAsync(ct);
    }
}
