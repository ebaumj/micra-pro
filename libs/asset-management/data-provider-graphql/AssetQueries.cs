using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
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
    ) =>
        (await assetService.Assets.FirstAsync().ToTask(ct))
            .Where(a => a is { IsAvailableRemotely: true, IsAvailableLocally: true })
            .Select(a => a.ToApi())
            .ToList();

    [RequiredPermissions([Permission.ReadAssets])]
    public static async Task<List<AssetUploadQueryApi>> GetUnfinishedAssets(
        [Service] IAssetService assetService,
        CancellationToken ct
    ) =>
        (
            await Task.WhenAll(
                (await assetService.Assets.FirstAsync().ToTask(ct))
                    .Where(a => !a.IsAvailableRemotely)
                    .Select(a => assetService.GetAssetUploadQueryAsync(a.Id, ct))
            )
        )
            .Select(q => q.ToApi())
            .ToList();

    [RequiredPermissions([Permission.ReadAssets])]
    public static Task<bool> GetIsAssetPolling(
        [Service] IAssetService assetService,
        Guid assetId,
        CancellationToken ct
    ) => assetService.IsAssetPolling(assetId).FirstAsync().ToTask(ct);
}
