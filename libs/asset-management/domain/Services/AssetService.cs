using MicraPro.AssetManagement.DataDefinition;
using MicraPro.AssetManagement.Domain.AssetAccess;
using MicraPro.AssetManagement.Domain.StorageAccess;
using MicraPro.AssetManagement.Domain.ValueObjects;

namespace MicraPro.AssetManagement.Domain.Services;

public class AssetService(
    IAssetRepository assetRepository,
    IAssetDirectoryService assetDirectoryService,
    IRemoteAssetService remoteAssetService
) : IAssetService
{
    private const string DefaultFileType = "txt";

    public async Task<IEnumerable<IAsset>> ReadAssetsAsync(CancellationToken ct)
    {
        var assets = await assetRepository.GetAllAsync(ct);
        var files = await assetDirectoryService.GetFilesAsync(ct);
        var remoteAssets = remoteAssetService.AvailableAssets;
        return assets.Select(a => new Asset(
            a.Id,
            a.RelativePath,
            files.Contains(a.RelativePath),
            remoteAssets.Contains(a.Id)
        ));
    }

    public async Task<IAsset> ReadAssetAsync(Guid assetId, CancellationToken ct)
    {
        var assets = await assetRepository.GetByIdAsync(assetId, ct);
        var files = await assetDirectoryService.GetFilesAsync(ct);
        var remoteAssets = remoteAssetService.AvailableAssets;
        return new Asset(
            assets.Id,
            assets.RelativePath,
            files.Contains(assets.RelativePath),
            remoteAssets.Contains(assets.Id)
        );
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

    public async Task SyncAssets(CancellationToken ct)
    {
        var assets = await assetRepository.GetAllAsync(ct);
        await remoteAssetService.FetchRemoteAssets(ct);
        var remoteAssets = remoteAssetService.AvailableAssets.ToArray();
        var localAssets = (await assetDirectoryService.GetFilesAsync(ct)).ToArray();
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
    }

    private async Task FetchAsset(AssetDb asset, CancellationToken ct)
    {
        var remoteAsset = await remoteAssetService.ReadRemoteAssetAsync(asset.Id, ct);
        var path =
            $"{Path.GetFileNameWithoutExtension(asset.RelativePath)}.{remoteAsset.FileEnding}";
        asset.RelativePath = path;
        await assetDirectoryService.WriteFileAsync(path, remoteAsset.Data, ct);
    }
}
