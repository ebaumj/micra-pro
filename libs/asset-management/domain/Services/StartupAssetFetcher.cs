using MicraPro.AssetManagement.DataDefinition;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MicraPro.AssetManagement.Domain.Services;

public class StartupAssetFetcher(IServiceScopeFactory serviceScopeFactory) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) =>
        serviceScopeFactory
            .CreateScope()
            .ServiceProvider.GetRequiredService<IAssetService>()
            .SyncAssets(cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
