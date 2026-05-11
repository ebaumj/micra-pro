namespace MicraPro.AssetManagement.DataDefinition;

public interface IAssetService
{
    IObservable<IEnumerable<IAsset>> Assets { get; }
    Task<IAssetUploadQuery> CreateAssetAsync(CancellationToken ct);
    Task<IAssetUploadQuery> GetAssetUploadQueryAsync(Guid assetId, CancellationToken ct);
    Task PollAssetAsync(Guid assetId, TimeSpan timeout, CancellationToken ct);
    IObservable<bool> IsAssetPolling(Guid assetId);
    Task BackupAssetsAsync(
        string sftpServer,
        string directory,
        string username,
        string password,
        CancellationToken ct
    );
    Task RestoreAssetsAsync(
        string sftpServer,
        string directory,
        string username,
        string password,
        CancellationToken ct
    );
}
