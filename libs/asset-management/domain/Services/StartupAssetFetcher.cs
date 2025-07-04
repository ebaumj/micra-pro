using System.Reactive.Disposables;
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
    private IDisposable _subscription = Disposable.Empty;

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
            _subscription = Observable
                .FromAsync(async ct => await SyncUntilSuccessAsync(ct))
                .Subscribe(_ => { });
    }

    private async Task SyncUntilSuccessAsync(CancellationToken ct)
    {
        while (
            !ct.IsCancellationRequested
            && !await serviceScopeFactory
                .CreateScope()
                .ServiceProvider.GetRequiredService<IAssetManagementService>()
                .SyncAssets(ct)
        )
            await Task.Delay(TimeSpan.FromSeconds(3), ct);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _subscription.Dispose();
        return Task.CompletedTask;
    }
}
