using MicraPro.Shared.Domain;

namespace MicraPro.Shared.DataProviderGraphQl;

[MutationType]
public static class SystemMutations
{
    public static Task<bool> Shutdown([Service] ISystemService service, CancellationToken ct) =>
        service.ShutdownAsync(ct);

    public static Task<bool> Reboot([Service] ISystemService service, CancellationToken ct) =>
        service.RebootAsync(ct);

    public static Task<bool> ConnectWifi(
        [Service] ISystemService service,
        string ssid,
        string? password,
        CancellationToken ct
    ) => service.ConnectWifiAsync(ssid, password, ct);
}
