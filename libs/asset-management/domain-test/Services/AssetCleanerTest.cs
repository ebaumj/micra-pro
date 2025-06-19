using System.Reactive.Linq;
using MicraPro.AssetManagement.DataDefinition;
using MicraPro.AssetManagement.Domain.Interfaces;
using MicraPro.AssetManagement.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace MicraPro.AssetManagement.Domain.Test.Services;

public class AssetCleanerTest
{
    private record DummyAsset(Guid Id) : IAsset
    {
        public string Path => throw new NotImplementedException();
        public bool IsAvailableLocally => throw new NotImplementedException();
        public bool IsAvailableRemotely => throw new NotImplementedException();
    }

    [Fact]
    public async Task CleanupAssetsAsyncTest()
    {
        var asset1 = new DummyAsset(Guid.NewGuid());
        var asset2 = new DummyAsset(Guid.NewGuid());
        var asset3 = new DummyAsset(Guid.NewGuid());

        var asset1ConsumerMock = new Mock<IAssetConsumer>();
        asset1ConsumerMock
            .Setup(m => m.GetAssetsAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IEnumerable<Guid>>([asset1.Id]));
        var asset2ConsumerMock = new Mock<IAssetConsumer>();
        asset2ConsumerMock
            .Setup(m => m.GetAssetsAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IEnumerable<Guid>>([asset2.Id]));

        var assetServiceMock = new Mock<IAssetManagementService>();
        assetServiceMock
            .Setup(m => m.Assets)
            .Returns(Observable.Return<IEnumerable<IAsset>>([asset1, asset2, asset3]));
        assetServiceMock
            .Setup(m => m.RemoveAssetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns((Guid id, CancellationToken _) => Task.FromResult(id));

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(m => m.GetService(typeof(IEnumerable<IAssetConsumer>)))
            .Returns(
                (IEnumerable<IAssetConsumer>)[asset1ConsumerMock.Object, asset2ConsumerMock.Object]
            );

        var service = new AssetCleaner(
            serviceProviderMock.Object,
            assetServiceMock.Object,
            Mock.Of<ILogger<AssetCleaner>>()
        );
        await service.CleanupAssetsAsync(CancellationToken.None);
        assetServiceMock.Verify(m => m.Assets);
        assetServiceMock.Verify(
            m => m.RemoveAssetAsync(asset3.Id, It.IsAny<CancellationToken>()),
            Times.Once
        );
        assetServiceMock.VerifyNoOtherCalls();
    }
}
