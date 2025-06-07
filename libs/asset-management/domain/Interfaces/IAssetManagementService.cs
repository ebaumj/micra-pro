using MicraPro.AssetManagement.DataDefinition;

namespace MicraPro.AssetManagement.Domain.Interfaces;

public interface IAssetManagementService
{
    IObservable<IEnumerable<IAsset>> Assets { get; }
    Task<Guid> RemoveAssetAsync(Guid assetId, CancellationToken ct);
    Task<bool> SyncAssets(CancellationToken ct);
}
