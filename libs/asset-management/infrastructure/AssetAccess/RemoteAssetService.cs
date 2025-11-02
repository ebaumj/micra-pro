using System.Text.Json;
using MicraPro.AssetManagement.Domain.AssetAccess;
using MicraPro.AssetManagement.Infrastructure.Interfaces;
using MicraPro.AssetManagement.Infrastructure.ValueObjects;
using Microsoft.Extensions.Options;

namespace MicraPro.AssetManagement.Infrastructure.AssetAccess;

public class RemoteAssetService(
    ITokenCreatorService tokenCreatorService,
    IHttpClientWrapperFactory clientFactory,
    IAssetServerDomainProvider domainProvider
) : IRemoteAssetService
{
    private const string EndpointAll = "api/assets";
    private const string EndpointUpload = "upload";

    private string EndpointId(Guid id) => $"{EndpointAll}/{id.ToString()}";

    private List<Guid> _remoteAssets = [];
    public IEnumerable<Guid> AvailableAssets => _remoteAssets;

    public async Task FetchRemoteAssetsAsync(CancellationToken ct)
    {
        var client = clientFactory.CreateClient(tokenCreatorService.GenerateAccessToken());
        var response = await client.MakeGetRequestAsync(
            $"{domainProvider.AssetServerLocalDomain}/{EndpointAll}",
            ct
        );
        client.Dispose();
        var assets = JsonSerializer.Deserialize<AllAssetsPayload>(response)!;
        _remoteAssets = assets.Assets.Select(Guid.Parse).ToList();
    }

    public async Task<(byte[] Data, string FileEnding)> ReadRemoteAssetAsync(
        Guid assetId,
        CancellationToken ct
    )
    {
        var client = clientFactory.CreateClient(tokenCreatorService.GenerateAccessToken(assetId));
        var response = await client.MakeGetRequestAsync(
            $"{domainProvider.AssetServerLocalDomain}/{EndpointId(assetId)}",
            ct
        );
        client.Dispose();
        var asset = JsonSerializer.Deserialize<AssetPayload>(response)!;
        return (Convert.FromBase64String(asset.DataBase64), asset.FileExtension);
    }

    public async Task RemoveRemoteAssetAsync(Guid assetId, CancellationToken ct)
    {
        var client = clientFactory.CreateClient(tokenCreatorService.GenerateAccessToken(assetId));
        await client.MakeDeleteRequestAsync(
            $"{domainProvider.AssetServerLocalDomain}/{EndpointId(assetId)}",
            ct
        );
        client.Dispose();
        _remoteAssets = _remoteAssets.Where(a => a != assetId).ToList();
    }

    public Task<string> CreateAssetUploadPathAsync(Guid assetId, CancellationToken ct) =>
        Task.FromResult(
            $"{domainProvider.AssetServerExternDomain}/{EndpointUpload}/{assetId.ToString()}?token={tokenCreatorService.GenerateUploadAccessToken(assetId)}"
        );
}
