using MicraPro.Shared.Domain;

namespace MicraPro.Shared.DataProviderGraphQl;

[MutationType]
public static class ConfigurationMutations
{
    public static async Task<string> WriteConfiguration(
        [Service] IConfigurationRepository service,
        string key,
        string jsonValue,
        CancellationToken ct
    )
    {
        await service.AddOrUpdateAsync(key, jsonValue, ct);
        return jsonValue;
    }

    public static async Task<string> DeleteConfiguration(
        [Service] IConfigurationRepository service,
        string key,
        CancellationToken ct
    )
    {
        await service.DeleteAsync(key, ct);
        return key;
    }
}
