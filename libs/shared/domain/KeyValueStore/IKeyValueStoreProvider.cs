namespace MicraPro.Shared.Domain.KeyValueStore;

public interface IKeyValueStoreProvider
{
    IKeyValueStore GetKeyValueStore(string storeNamespace);
}
