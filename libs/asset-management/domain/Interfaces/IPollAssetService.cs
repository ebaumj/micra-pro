namespace MicraPro.AssetManagement.Domain.Interfaces;

public interface IPollAssetService
{
    void StartPollAsset(Guid assetId, TimeSpan timeout);
    IObservable<bool> IsPollingAsset(Guid assetId);
}
