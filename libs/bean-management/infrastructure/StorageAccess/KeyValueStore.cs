using MicraPro.BeanManagement.Domain.StorageAccess;

namespace MicraPro.BeanManagement.Infrastructure.StorageAccess;

internal class KeyValueStore(
    MicraPro.Shared.Domain.KeyValueStore.IKeyValueStoreProvider keyValueStoreProvider
) : IKeyValueStore
{
    private static readonly string Namespace =
        $"{typeof(KeyValueStore).Namespace!}.{nameof(KeyValueStore)}";
    private readonly Shared.Domain.KeyValueStore.IKeyValueStore _store =
        keyValueStoreProvider.GetKeyValueStore(Namespace);

    public Task<string?> TryGetAsync(string key, CancellationToken ct) =>
        _store.TryGetAsync(key, ct);

    public Task AddOrUpdateAsync(string key, string jsonValue, CancellationToken ct) =>
        _store.AddOrUpdateAsync(key, jsonValue, ct);

    public Task DeleteAsync(string key, CancellationToken ct) => _store.DeleteAsync(key, ct);
}
