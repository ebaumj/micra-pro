using MicraPro.Auth.DataDefinition;
using MicraPro.Shared.Domain;

namespace MicraPro.Shared.DataProviderGraphQl;

[QueryType]
public static class ConfigurationQueries
{
    [RequiredPermissions([Permission.ReadConfiguration])]
    public static Task<string> ReadConfiguration(
        [Service] IConfigurationRepository service,
        string key,
        CancellationToken ct
    ) => service.GetAsync(key, ct);
}
