using MicraPro.AssetManagement.DataDefinition;
using MicraPro.AssetManagement.DataProviderGraphQl.Types;
using MicraPro.Auth.DataDefinition;

namespace MicraPro.AssetManagement.DataProviderGraphQl;

[QueryType]
public static class AssetQueries
{
    [RequiredPermissions([Permission.ReadAssets])]
    public static async Task<List<AssetApi>> GetAvailableAssets(
        [Service] IAssetService assetService,
        CancellationToken ct
    ) => (await assetService.ReadAssetsAsync(ct)).Select(a => a.ToApi()).ToList();

    [RequiredPermissions([Permission.ReadAssets])]
    public static async Task<List<AssetApi>> SyncAssets(
        [Service] IAssetService assetService,
        CancellationToken ct
    )
    {
        await assetService.SyncAssets(ct);
        return (await assetService.ReadAssetsAsync(ct)).Select(a => a.ToApi()).ToList();
    }
}
