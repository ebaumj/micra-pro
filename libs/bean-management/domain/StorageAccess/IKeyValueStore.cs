namespace MicraPro.BeanManagement.Domain.StorageAccess;

public interface IKeyValueStore
{
    Task<string?> TryGetAsync(string key, CancellationToken ct);
    Task AddOrUpdateAsync(string key, string jsonValue, CancellationToken ct);
    Task DeleteAsync(string key, CancellationToken ct);
}
