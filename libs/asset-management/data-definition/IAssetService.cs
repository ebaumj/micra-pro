namespace MicraPro.AssetManagement.DataDefinition;

public interface IAssetService
{
    Task<IEnumerable<IAsset>> ReadAssetsAsync(CancellationToken ct);
    Task<IAsset> ReadAssetAsync(Guid assetId, CancellationToken ct);
    Task<IAssetUploadQuery> CreateAssetAsync(CancellationToken ct);
    Task<IAssetUploadQuery> GetAssetUploadQueryAsync(Guid assetId, CancellationToken ct);
    Task<Guid> RemoveAssetAsync(Guid assetId, CancellationToken ct);
    Task SyncAssets(CancellationToken ct);
}
