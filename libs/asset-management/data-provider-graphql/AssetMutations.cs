using MicraPro.AssetManagement.DataDefinition;
using MicraPro.AssetManagement.DataProviderGraphQl.Types;

namespace MicraPro.AssetManagement.DataProviderGraphQl;

[MutationType]
public static class AssetMutations
{
    public static async Task<AssetUploadQueryApi> CreateAsset(
        [Service] IAssetService assetService,
        CancellationToken ct
    ) => (await assetService.CreateAssetAsync(ct)).ToApi();

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
