using MicraPro.Shared.Domain;
using Microsoft.Extensions.Logging;

namespace MicraPro.Shared.Infrastructure;

public class SystemServiceDummy(ILogger<SystemServiceDummy> logger) : ISystemService
{
    private string? _wifi;

    public Task<bool> ShutdownAsync(CancellationToken ct)
    {
        logger.LogInformation("shutdown");
        return Task.FromResult(false);
    }

    public Task<bool> RebootAsync(CancellationToken ct)
    {
        logger.LogInformation("reboot");
        return Task.FromResult(false);
    }

    public async Task<string?> GetConnectedWifiAsync(CancellationToken ct)
    {
        await Task.Delay(500, ct);
        return _wifi;
    }

    public async Task<ISystemService.Wifi[]> ScanWifiAsync(CancellationToken ct)
    {
        await Task.Delay(2000, ct);
        return
        [
            new ISystemService.Wifi("Dummy 1", true),
            new ISystemService.Wifi("Dummy 2", false),
        ];
    }

    public async Task<bool> ConnectWifiAsync(string ssid, string? password, CancellationToken ct)
    {
        await Task.Delay(500, ct);
        _wifi = ssid switch
        {
            "Dummy 1" when password == "dummy" => "Dummy 1",
            "Dummy 2" => "Dummy 2",
            _ => null,
        };
        return _wifi != null;
    }
}
