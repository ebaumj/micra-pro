using MicraPro.Shared.Domain.KeyValueStore;

namespace MicraPro.Shared.Infrastructure.KeyValueStore;

internal class KeyValueStore(KeyValueStoreBase store, string storeNamespace) : IKeyValueStore
{
    public Task<string?> TryGetAsync(string key, CancellationToken ct) =>
        store.TryGetAsync(storeNamespace, key, ct);

    public Task AddOrUpdateAsync(string key, string jsonValue, CancellationToken ct) =>
        store.AddOrUpdateAsync(storeNamespace, key, jsonValue, ct);

    public Task DeleteAsync(string key, CancellationToken ct) =>
        store.DeleteAsync(storeNamespace, key, ct);
}
