namespace MicraPro.AssetManagement.Domain.Interfaces;

public interface IAssetCleaner
{
    Task CleanupAssetsAsync(CancellationToken ct);
}
