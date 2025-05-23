namespace MicraPro.Shared.Domain;

public interface IConfigurationRepository
{
    Task<string> GetAsync(string key, CancellationToken ct);
    Task AddOrUpdateAsync(string key, string jsonValue, CancellationToken ct);
    Task DeleteAsync(string key, CancellationToken ct);
}
