using MicraPro.Shared.Domain.KeyValueStore;

namespace MicraPro.Shared.Infrastructure.KeyValueStore;

internal class KeyValueStoreProvider(KeyValueStoreBase store) : IKeyValueStoreProvider
{
    public IKeyValueStore GetKeyValueStore(string storeNamespace) =>
        new KeyValueStore(store, storeNamespace);
}
