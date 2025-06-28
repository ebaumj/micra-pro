using System.Reactive.Linq;
using MicraPro.AssetManagement.Domain.AssetAccess;
using MicraPro.AssetManagement.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MicraPro.AssetManagement.Domain.Services;

public class StartupAssetFetcher(
    IServiceScopeFactory serviceScopeFactory,
    IAssetDirectoryService assetDirectoryService
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await assetDirectoryService.ReadFilesAsync(cancellationToken);
        var serviceProvider = serviceScopeFactory.CreateScope().ServiceProvider;
        var success = await serviceProvider
            .GetRequiredService<IAssetManagementService>()
            .SyncAssets(cancellationToken);
        await serviceProvider
            .GetRequiredService<IAssetCleaner>()
            .CleanupAssetsAsync(cancellationToken);
        if (!success)
            Observable
                .Timer(TimeSpan.FromSeconds(3))
                .Select(_ =>
                    Observable.FromAsync(async ct =>
                        await serviceProvider
                            .GetRequiredService<IAssetManagementService>()
                            .SyncAssets(ct)
                    )
                )
                .Merge()
                .Subscribe(_ => { });
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
