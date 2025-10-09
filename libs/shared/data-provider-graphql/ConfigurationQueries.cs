using MicraPro.Shared.Domain;

namespace MicraPro.Shared.DataProviderGraphQl;

[QueryType]
public static class ConfigurationQueries
{
    public static async Task<string> ReadConfiguration(
        [Service] IConfigurationRepository service,
        string key,
        CancellationToken ct
    )
    {
        try
        {
            return await service.GetAsync(key, ct);
        }
        catch (InvalidOperationException)
        {
            throw new ConfigDoesNotExistException(key);
        }
    }
}
