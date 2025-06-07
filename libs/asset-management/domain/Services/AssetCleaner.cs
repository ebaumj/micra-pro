using MicraPro.AssetManagement.DataDefinition;
using MicraPro.AssetManagement.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MicraPro.AssetManagement.Domain.Services;

public class AssetCleaner(
    IServiceProvider serviceProvider,
    IAssetService assetService,
    ILogger<AssetCleaner> logger
) : IAssetCleaner
{
    public async Task CleanupAssetsAsync(CancellationToken ct)
    {
        var usedAssets = (
            await Task.WhenAll(
                serviceProvider.GetServices<IAssetConsumer>().Select(s => s.GetAssetsAsync(ct))
            )
        ).SelectMany(id => id);
        try
        {
            await Task.WhenAll(
                (await assetService.ReadAssetsAsync(ct))
                    .Select(a => a.Id)
                    .Where(a => !usedAssets.Contains(a))
                    .Select(a => assetService.RemoveAssetAsync(a, ct))
            );
            logger.LogInformation("Asset cleanup completed");
        }
        catch
        {
            logger.LogCritical("Asset cleanup failed");
        }
    }
}
