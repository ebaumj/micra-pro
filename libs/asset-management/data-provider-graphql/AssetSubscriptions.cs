using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using HotChocolate.Execution;
using MicraPro.AssetManagement.DataDefinition;
using MicraPro.AssetManagement.DataProviderGraphQl.Types;
using MicraPro.Auth.DataDefinition;
using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.AssetManagement.DataProviderGraphQl;

[SubscriptionType]
public static class AssetSubscriptions
{
    [Subscribe(With = nameof(SubscribeToAvailableAssetsChanged))]
    [RequiredPermissions([Permission.ReadAssets])]
    public static List<AssetApi> AvailableAssetsChanged([EventMessage] List<AssetApi> assets) =>
        assets;

    [Subscribe(With = nameof(SubscribeToUnfinishedAssetsChanged))]
    [RequiredPermissions([Permission.ReadAssets])]
    public static List<AssetUploadQueryApi> UnfinishedAssetsChanged(
        [EventMessage] List<AssetUploadQueryApi> assets
    ) => assets;

    [Subscribe(With = nameof(SubscribeToIsAssetPollingChanged))]
    [RequiredPermissions([Permission.ReadAssets])]
    public static bool IsAssetPolling([EventMessage] bool isPolling) => isPolling;

    public static ValueTask<ISourceStream<List<AssetApi>>> SubscribeToAvailableAssetsChanged(
        [Service] IAssetService assetService
    ) =>
        ValueTask.FromResult(
            assetService
                .Assets.Select(a =>
                    a.Where(asset =>
                            asset is { IsAvailableRemotely: true, IsAvailableLocally: true }
                        )
                        .Select(asset => asset.ToApi())
                        .ToList()
                )
                .ToSourceStream()
        );

    public static ValueTask<
        ISourceStream<List<AssetUploadQueryApi>>
    > SubscribeToUnfinishedAssetsChanged([Service] IAssetService assetService) =>
        ValueTask.FromResult(
            assetService
                .Assets.Select(assets =>
                    Observable.FromAsync(async ct =>
                        (
                            await Task.WhenAll(
                                (await assetService.Assets.FirstAsync().ToTask(ct))
                                    .Where(a => !a.IsAvailableRemotely)
                                    .Select(a => assetService.GetAssetUploadQueryAsync(a.Id, ct))
                            )
                        )
                            .Select(asset => asset.ToApi())
                            .ToList()
                    )
                )
                .Merge()
                .ToSourceStream()
        );

    public static ValueTask<ISourceStream<bool>> SubscribeToIsAssetPollingChanged(
        [Service] IAssetService assetService,
        Guid assetId
    ) => ValueTask.FromResult(assetService.IsAssetPolling(assetId).ToSourceStream());
}
