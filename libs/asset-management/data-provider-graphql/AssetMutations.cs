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

    [RequiredPermissions([Permission.WriteAssets])]
    public static Task<Guid> RemoveAsset(
        [Service] IAssetService assetService,
        Guid assetId,
        CancellationToken ct
    ) => assetService.RemoveAssetAsync(assetId, ct);
}
