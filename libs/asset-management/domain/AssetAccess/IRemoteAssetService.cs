namespace MicraPro.AssetManagement.Domain.AssetAccess;

public interface IRemoteAssetService
{
    IEnumerable<Guid> AvailableAssets { get; }
    Task FetchRemoteAssets(CancellationToken ct);
    Task<(byte[] Data, string FileEnding)> ReadRemoteAssetAsync(Guid assetId, CancellationToken ct);
    Task RemoveRemoteAssetAsync(Guid assetId, CancellationToken ct);
    Task<string> CreateAssetUploadPathAsync(Guid assetId, CancellationToken ct);
}
