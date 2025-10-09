using MicraPro.Shared.Domain;

namespace MicraPro.Shared.DataProviderGraphQl;

[QueryType]
public static class SystemQueries
{
    public static Task<string?> GetConnectedWifi(
        [Service] ISystemService service,
        CancellationToken ct
    ) => service.GetConnectedWifiAsync(ct);

    public static Task<ISystemService.Wifi[]> ScanWifi(
        [Service] ISystemService service,
        CancellationToken ct
    ) => service.ScanWifiAsync(ct);
}
