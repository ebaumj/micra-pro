using MicraPro.AssetManagement.DataDefinition;
using MicraPro.AssetManagement.Domain.AssetAccess;
using MicraPro.AssetManagement.Domain.Interfaces;
using MicraPro.AssetManagement.Domain.StorageAccess;
using MicraPro.AssetManagement.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace MicraPro.AssetManagement.Domain.Services;

public class AssetService(
    IAssetRepository assetRepository,
    IAssetDirectoryService assetDirectoryService,
    IRemoteAssetService remoteAssetService,
    AssetStateService assetStateService,
    IPollAssetService pollAssetService,
    ILogger<AssetService> logger
) : IAssetService, IAssetManagementService
{
    private const string DefaultFileType = "txt";

    public IObservable<IEnumerable<IAsset>> Assets => assetStateService.Assets;

    private async Task<IEnumerable<IAsset>> ReadAssetsAsync(CancellationToken ct)
    {
        var assets = await assetRepository.GetAllAsync(ct);
        var files = assetDirectoryService.Files;
        var remoteAssets = remoteAssetService.AvailableAssets;
        return assets.Select(a => new Asset(
            a.Id,
            assetDirectoryService.LocalServerPath(a.RelativePath),
            files.Contains(a.RelativePath),
            remoteAssets.Contains(a.Id)
        ));
    }

    public async Task<IAssetUploadQuery> CreateAssetAsync(CancellationToken ct)
    {
        var asset = new AssetDb(
            $"{assetDirectoryService.CreateRandomFileNameWithoutExtension()}.{DefaultFileType}"
        );
        await assetRepository.AddAsync(asset, ct);
        await assetRepository.SaveAsync(ct);
        return new AssetUploadQuery(
            asset.Id,
            await remoteAssetService.CreateAssetUploadPathAsync(asset.Id, ct)
        );
    }

    public async Task<IAssetUploadQuery> GetAssetUploadQueryAsync(
        Guid assetId,
        CancellationToken ct
    ) =>
        new AssetUploadQuery(
            assetId,
            await remoteAssetService.CreateAssetUploadPathAsync(assetId, ct)
        );

    public async Task<Guid> RemoveAssetAsync(Guid assetId, CancellationToken ct)
    {
        var asset = await assetRepository.GetByIdAsync(assetId, ct);
        await Task.WhenAll(
            assetDirectoryService.RemoveFileAsync(asset.RelativePath, ct),
            remoteAssetService.RemoveRemoteAssetAsync(assetId, ct),
            assetRepository.DeleteAsync(assetId, ct)
        );
        await assetRepository.SaveAsync(ct);
        return assetId;
    }

    public async Task<bool> SyncAssets(CancellationToken ct)
    {
        var assets = await assetRepository.GetAllAsync(ct);
        try
        {
            await remoteAssetService.FetchRemoteAssetsAsync(ct);
        }
        catch
        {
            logger.LogError("Failed to read remote assets");
            return false;
        }
        var remoteAssets = remoteAssetService.AvailableAssets.ToArray();
        var localAssets = assetDirectoryService.Files.ToArray();
        await Task.WhenAll(
            assets
                .Where(a => !localAssets.Contains(a.RelativePath) && remoteAssets.Contains(a.Id))
                .Select(a => FetchAsset(a, ct))
        );
        await assetRepository.SaveAsync(ct);
        await Task.WhenAll(
            remoteAssets
                .Where(a => assets.FirstOrDefault(local => local.Id == a) == null)
                .Select(a => remoteAssetService.RemoveRemoteAssetAsync(a, ct))
        );
        await Task.WhenAll(
            localAssets
                .Where(a => assets.FirstOrDefault(local => local.RelativePath == a) == null)
                .Select(a => assetDirectoryService.RemoveFileAsync(a, ct))
        );
        assetStateService.Assets.OnNext(await ReadAssetsAsync(ct));
        logger.LogInformation("Asset synchronization completed");
        return true;
    }

    public Task PollAssetAsync(Guid assetId, TimeSpan timeout, CancellationToken _)
    {
        pollAssetService.StartPollAsset(assetId, timeout);
        return Task.CompletedTask;
    }

    public IObservable<bool> IsAssetPolling(Guid assetId) =>
        pollAssetService.IsPollingAsset(assetId);

    private async Task FetchAsset(AssetDb asset, CancellationToken ct)
    {
        var remoteAsset = await remoteAssetService.ReadRemoteAssetAsync(asset.Id, ct);
        var path = Path.ChangeExtension(asset.RelativePath, remoteAsset.FileEnding);
        asset.RelativePath = path;
        await assetDirectoryService.WriteFileAsync(path, remoteAsset.Data, ct);
    }
}
