using MicraPro.AssetManagement.Domain.AssetAccess;
using MicraPro.AssetManagement.Domain.Interfaces;
using MicraPro.AssetManagement.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace MicraPro.AssetManagement.Domain.Test.Services;

public class StartupAssetFetcherTest
{
    [Fact]
    public async Task FetchAssetsOnStartupTest()
    {
        var assetManagementServiceMock = new Mock<IAssetManagementService>();
        assetManagementServiceMock
            .Setup(m => m.SyncAssets(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(true));
        var assetCleanerMock = new Mock<IAssetCleaner>();
        assetCleanerMock
            .Setup(m => m.CleanupAssetsAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var assetDirectoryServiceMock = new Mock<IAssetDirectoryService>();
        assetDirectoryServiceMock
            .Setup(m => m.ReadFilesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(m => m.GetService(typeof(IAssetManagementService)))
            .Returns(assetManagementServiceMock.Object);
        serviceProviderMock
            .Setup(m => m.GetService(typeof(IAssetCleaner)))
            .Returns(assetCleanerMock.Object);
        var serviceScopeMock = new Mock<IServiceScope>();
        serviceScopeMock.Setup(m => m.ServiceProvider).Returns(serviceProviderMock.Object);
        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        serviceScopeFactoryMock.Setup(m => m.CreateScope()).Returns(serviceScopeMock.Object);
        var service = new StartupAssetFetcher(
            serviceScopeFactoryMock.Object,
            assetDirectoryServiceMock.Object
        );
        await service.StartAsync(CancellationToken.None);
        assetManagementServiceMock.Verify(
            m => m.SyncAssets(It.IsAny<CancellationToken>()),
            Times.Once
        );
        assetCleanerMock.Verify(
            m => m.CleanupAssetsAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
        assetDirectoryServiceMock.Verify(
            m => m.ReadFilesAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
        await service.StopAsync(CancellationToken.None);
        assetManagementServiceMock.VerifyNoOtherCalls();
        assetCleanerMock.VerifyNoOtherCalls();
        assetDirectoryServiceMock.VerifyNoOtherCalls();
    }
}
