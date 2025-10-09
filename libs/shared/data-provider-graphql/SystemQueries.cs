using MicraPro.Auth.DataDefinition;
using MicraPro.Shared.Domain;

namespace MicraPro.Shared.DataProviderGraphQl;

[QueryType]
public static class SystemQueries
{
    [RequiredPermissions([Permission.SystemAccess])]
    public static Task<string?> GetConnectedWifi(
        [Service] ISystemService service,
        CancellationToken ct
    ) => service.GetConnectedWifiAsync(ct);

    [RequiredPermissions([Permission.SystemAccess])]
    public static Task<ISystemService.Wifi[]> ScanWifi(
        [Service] ISystemService service,
        CancellationToken ct
    ) => service.ScanWifiAsync(ct);
}
