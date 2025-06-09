using System.Reactive.Linq;
using System.Reactive.Subjects;
using MicraPro.AssetManagement.Domain.AssetAccess;
using MicraPro.AssetManagement.Domain.Interfaces;
using MicraPro.AssetManagement.Domain.StorageAccess;
using MicraPro.AssetManagement.Domain.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.AssetManagement.Domain.Services;

public class PollAssetService(
    IServiceScopeFactory serviceScopeFactory,
    IAssetDirectoryService assetDirectoryService,
    IRemoteAssetService remoteAssetService,
    AssetStateService assetStateService
) : IPollAssetService
{
    private static readonly TimeSpan PollPeriod = TimeSpan.FromSeconds(5);

    private record EndTime(Guid AssetId, DateTime Time);

    private readonly BehaviorSubject<EndTime[]> _assetsEndTime = new([]);

    private async Task<(bool Success, int Size)> PollAsset(
        Guid assetId,
        int size,
        CancellationToken ct
    )
    {
        try
        {
            var repository = serviceScopeFactory
                .CreateScope()
                .ServiceProvider.GetRequiredService<IAssetRepository>();
            var asset = await repository.GetByIdAsync(assetId, ct);
            await remoteAssetService.FetchRemoteAssets(ct);
            if (!remoteAssetService.AvailableAssets.Contains(assetId))
                return (false, 0);
            var remoteAsset = await remoteAssetService.ReadRemoteAssetAsync(asset.Id, ct);
            if (remoteAsset.Data.Length != size)
                return (false, remoteAsset.Data.Length);
            var path = Path.ChangeExtension(asset.RelativePath, remoteAsset.FileEnding);
            asset.RelativePath = path;
            await repository.SaveAsync(ct);
            await assetDirectoryService.WriteFileAsync(path, remoteAsset.Data, ct);
            var assets = await repository.GetAllAsync(ct);
            assetStateService.Assets.OnNext(
                assets.Select(a => new Asset(
                    a.Id,
                    assetDirectoryService.LocalServerPath(a.RelativePath),
                    assetDirectoryService.Files.Contains(a.RelativePath),
                    true
                ))
            );
            return (true, remoteAsset.Data.Length);
        }
        catch
        {
            return (false, 0);
        }
    }

    private void RecursiveAssetPoll(Guid assetId, int size)
    {
        Observable
            .Timer(PollPeriod)
            .Select(_ => Observable.FromAsync(async (ct) => await PollAsset(assetId, size, ct)))
            .Merge()
            .Subscribe(
                (state) =>
                {
                    if (DateTime.UtcNow < AssetPollEndTime(assetId) && !state.Success)
                        RecursiveAssetPoll(assetId, state.Size);
                    else
                        RemoveAssetPollEndTime(assetId);
                }
            );
    }

    public void StartPollAsset(Guid assetId, TimeSpan timeout)
    {
        var startPoll = _assetsEndTime.Value.FirstOrDefault(v => v.AssetId == assetId) == null;
        SetAssetPollEndTime(assetId, DateTime.UtcNow.Add(timeout));
        if (startPoll)
            RecursiveAssetPoll(assetId, 0);
    }

    public IObservable<bool> IsPollingAsset(Guid assetId) =>
        _assetsEndTime.Select(entries => entries.FirstOrDefault(v => v.AssetId == assetId) != null);

    private DateTime AssetPollEndTime(Guid assetId) =>
        _assetsEndTime.Value.FirstOrDefault(v => v.AssetId == assetId)?.Time ?? DateTime.UtcNow;

    private void SetAssetPollEndTime(Guid assetId, DateTime time) =>
        _assetsEndTime.OnNext(
            _assetsEndTime
                .Value.Where(v => v.AssetId != assetId)
                .Concat([new EndTime(assetId, time)])
                .ToArray()
        );

    private void RemoveAssetPollEndTime(Guid assetId) =>
        _assetsEndTime.OnNext(_assetsEndTime.Value.Where(v => v.AssetId != assetId).ToArray());
}
