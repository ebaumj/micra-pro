using MicraPro.AssetManagement.DataDefinition;
using MicraPro.AssetManagement.DataProviderGraphQl.Types;
using MicraPro.Auth.DataDefinition;

namespace MicraPro.AssetManagement.DataProviderGraphQl;

[MutationType]
public static class AssetMutations
{
    [RequiredPermissions([Permission.WriteAssets])]
    public static async Task<AssetUploadQueryApi> CreateAsset(
        [Service] IAssetService assetService,
        CancellationToken ct
    ) => (await assetService.CreateAssetAsync(ct)).ToApi();

    [RequiredPermissions([Permission.ReadAssets])]
    public static async Task<bool> PollAsset(
        [Service] IAssetService assetService,
        Guid assetId,
        TimeSpan timeout,
        CancellationToken ct
    )
    {
        await assetService.PollAssetAsync(assetId, timeout, ct);
        return true;
    }
}
