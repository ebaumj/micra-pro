using Microsoft.Extensions.Hosting;

namespace MicraPro.Shared.Infrastructure;

public class WifiEnableHostService(IWifiEnableService wifiEnableService) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await wifiEnableService.EnableWifi(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
