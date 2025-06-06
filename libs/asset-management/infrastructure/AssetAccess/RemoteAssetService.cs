using System.Text.Json;
using MicraPro.AssetManagement.Domain.AssetAccess;
using MicraPro.AssetManagement.Infrastructure.Interfaces;
using MicraPro.AssetManagement.Infrastructure.ValueObjects;
using Microsoft.Extensions.Options;

namespace MicraPro.AssetManagement.Infrastructure.AssetAccess;

public class RemoteAssetService(
    ITokenCreatorService tokenCreatorService,
    IOptions<AssetManagementInfrastructureOptions> options
) : IRemoteAssetService
{
    private const string EndpointAll = "api/assets";
    private const string EndpointUpload = "upload";

    private string EndpointId(Guid id) => $"{EndpointAll}/{id.ToString()}";

    private List<Guid> _remoteAssets = [];
    public IEnumerable<Guid> AvailableAssets => _remoteAssets;

    private HttpClient CreateClient()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                tokenCreatorService.GenerateAccessToken()
            );
        return client;
    }

    private HttpClient CreateClient(Guid assetId)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer",
                tokenCreatorService.GenerateAccessToken(assetId)
            );
        return client;
    }

    public async Task FetchRemoteAssets(CancellationToken ct)
    {
        var client = CreateClient();
        var response = await client.GetAsync(
            $"{options.Value.RemoteFileServerDomain}/{EndpointAll}",
            ct
        );
        client.Dispose();
        var assets = JsonSerializer.Deserialize<AllAssetsPayload>(
            await response.Content.ReadAsStringAsync(ct)
        )!;
        _remoteAssets = assets.Assets.Select(Guid.Parse).ToList();
    }

    public async Task<(byte[] Data, string FileEnding)> ReadRemoteAssetAsync(
        Guid assetId,
        CancellationToken ct
    )
    {
        var client = CreateClient(assetId);
        var response = await client.GetAsync(
            $"{options.Value.RemoteFileServerDomain}/{EndpointId(assetId)}",
            ct
        );
        client.Dispose();
        var asset = JsonSerializer.Deserialize<AssetPayload>(
            await response.Content.ReadAsStringAsync(ct)
        )!;
        return (Convert.FromBase64String(asset.DataBase64), asset.FileExtension);
    }

    public async Task RemoveRemoteAssetAsync(Guid assetId, CancellationToken ct)
    {
        var client = CreateClient(assetId);
        await client.DeleteAsync(
            $"{options.Value.RemoteFileServerDomain}/{EndpointId(assetId)}",
            ct
        );
        client.Dispose();
        _remoteAssets = _remoteAssets.Where(a => a != assetId).ToList();
    }

    public Task<string> CreateAssetUploadPathAsync(Guid assetId, CancellationToken ct) =>
        Task.FromResult(
            $"{options.Value.RemoteFileServerDomain}/{EndpointUpload}/{assetId.ToString()}?token={tokenCreatorService.GenerateUploadAccessToken(assetId)}"
        );
}
