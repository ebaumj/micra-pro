using MicraPro.Shared.Domain;

namespace MicraPro.Shared.DataProviderGraphQl;

[QueryType]
public static class SystemQueries
{
    public record SystemVersion(string Version, bool AllowUpdates);

    public static Task<string?> GetConnectedWifi(
        [Service] ISystemService service,
        CancellationToken ct
    ) => service.GetConnectedWifiAsync(ct);

    public static Task<ISystemService.Wifi[]> ScanWifi(
        [Service] ISystemService service,
        CancellationToken ct
    ) => service.ScanWifiAsync(ct);

    public static Task<SystemVersion> GetSystemVersion(
        [Service] ISystemService service,
        CancellationToken ct
    ) => Task.FromResult(new SystemVersion(service.SystemVersion, service.AllowUpdates));

    public static Task<bool> GetAllowUpdates(
        [Service] ISystemService service,
        CancellationToken ct
    ) => Task.FromResult(service.AllowUpdates);
}
