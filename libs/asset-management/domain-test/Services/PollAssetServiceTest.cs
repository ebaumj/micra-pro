using System.Reactive.Linq;
using MicraPro.AssetManagement.Domain.AssetAccess;
using MicraPro.AssetManagement.Domain.Services;
using MicraPro.AssetManagement.Domain.StorageAccess;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace MicraPro.AssetManagement.Domain.Test.Services;

public class PollAssetServiceTest
{
    [Fact]
    public async Task StartPollAssetSuccessTest()
    {
        AssetDb dummyAsset = new("SomePath.dummyBefore");
        byte[] dummyData = [1, 2, 3, 4, 5];
        const string dummyFileEnding = "dummyAfter";
        var assetRepositoryMock = new Mock<IAssetRepository>();
        assetRepositoryMock
            .Setup(m => m.GetByIdAsync(dummyAsset.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(dummyAsset));
        assetRepositoryMock
            .Setup(m => m.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        assetRepositoryMock
            .Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IReadOnlyCollection<AssetDb>>([dummyAsset]));
        var remoteAssetServiceMock = new Mock<IRemoteAssetService>();
        remoteAssetServiceMock
            .Setup(m => m.FetchRemoteAssetsAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        remoteAssetServiceMock
            .Setup(m => m.ReadRemoteAssetAsync(dummyAsset.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((Data: dummyData, FileEnding: dummyFileEnding)));
        remoteAssetServiceMock.Setup(m => m.AvailableAssets).Returns([dummyAsset.Id]);
        var assetDirectoryServiceMock = new Mock<IAssetDirectoryService>();
        assetDirectoryServiceMock
            .Setup(m =>
                m.WriteFileAsync(dummyAsset.RelativePath, dummyData, It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask);
        assetDirectoryServiceMock
            .Setup(m => m.LocalServerPath(dummyAsset.RelativePath))
            .Returns(() => "SomeLocalServerPath");
        assetDirectoryServiceMock.Setup(m => m.Files).Returns(() => [dummyAsset.RelativePath]);
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(m => m.GetService(typeof(IAssetRepository)))
            .Returns(assetRepositoryMock.Object);
        var serviceScopeMock = new Mock<IServiceScope>();
        serviceScopeMock.Setup(m => m.ServiceProvider).Returns(serviceProviderMock.Object);
        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        serviceScopeFactoryMock.Setup(m => m.CreateScope()).Returns(serviceScopeMock.Object);
        var service = new PollAssetService(
            serviceScopeFactoryMock.Object,
            assetDirectoryServiceMock.Object,
            remoteAssetServiceMock.Object,
            new AssetStateService()
        );

        var observerMock = new Mock<IObserver<bool>>();
        service.IsPollingAsset(dummyAsset.Id).Subscribe(observerMock.Object);
        observerMock.Verify(m => m.OnNext(false));
        service.StartPollAsset(dummyAsset.Id, TimeSpan.FromSeconds(10.2));
        observerMock.Verify(m => m.OnNext(true));
        await Task.Delay(TimeSpan.FromSeconds(10.1));
        assetRepositoryMock.Verify(m =>
            m.GetByIdAsync(dummyAsset.Id, It.IsAny<CancellationToken>())
        );
        assetRepositoryMock.Verify(m => m.SaveAsync(It.IsAny<CancellationToken>()));
        remoteAssetServiceMock.Verify(m => m.FetchRemoteAssetsAsync(It.IsAny<CancellationToken>()));
        remoteAssetServiceMock.Verify(m =>
            m.ReadRemoteAssetAsync(dummyAsset.Id, It.IsAny<CancellationToken>())
        );
        assetDirectoryServiceMock.Verify(m =>
            m.WriteFileAsync(
                $"SomePath.{dummyFileEnding}",
                dummyData,
                It.IsAny<CancellationToken>()
            )
        );
        Assert.False(await service.IsPollingAsset(dummyAsset.Id).FirstAsync());
    }

    [Fact]
    public async Task StartPollAssetNotAvailableRemotelyTest()
    {
        AssetDb dummyAsset = new("SomePath.dummyBefore");
        byte[] dummyData = [1, 2, 3, 4, 5];
        const string dummyFileEnding = "dummyAfter";
        var assetRepositoryMock = new Mock<IAssetRepository>();
        assetRepositoryMock
            .Setup(m => m.GetByIdAsync(dummyAsset.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(dummyAsset));
        assetRepositoryMock
            .Setup(m => m.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        assetRepositoryMock
            .Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IReadOnlyCollection<AssetDb>>([dummyAsset]));
        var remoteAssetServiceMock = new Mock<IRemoteAssetService>();
        remoteAssetServiceMock
            .Setup(m => m.FetchRemoteAssetsAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        remoteAssetServiceMock
            .Setup(m => m.ReadRemoteAssetAsync(dummyAsset.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((Data: dummyData, FileEnding: dummyFileEnding)));
        remoteAssetServiceMock.Setup(m => m.AvailableAssets).Returns([]);
        var assetDirectoryServiceMock = new Mock<IAssetDirectoryService>();
        assetDirectoryServiceMock
            .Setup(m =>
                m.WriteFileAsync(dummyAsset.RelativePath, dummyData, It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask);
        assetDirectoryServiceMock
            .Setup(m => m.LocalServerPath(dummyAsset.RelativePath))
            .Returns(() => "SomeLocalServerPath");
        assetDirectoryServiceMock.Setup(m => m.Files).Returns(() => [dummyAsset.RelativePath]);
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(m => m.GetService(typeof(IAssetRepository)))
            .Returns(assetRepositoryMock.Object);
        var serviceScopeMock = new Mock<IServiceScope>();
        serviceScopeMock.Setup(m => m.ServiceProvider).Returns(serviceProviderMock.Object);
        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        serviceScopeFactoryMock.Setup(m => m.CreateScope()).Returns(serviceScopeMock.Object);
        var service = new PollAssetService(
            serviceScopeFactoryMock.Object,
            assetDirectoryServiceMock.Object,
            remoteAssetServiceMock.Object,
            new AssetStateService()
        );

        var observerMock = new Mock<IObserver<bool>>();
        service.IsPollingAsset(dummyAsset.Id).Subscribe(observerMock.Object);
        observerMock.Verify(m => m.OnNext(false));
        service.StartPollAsset(dummyAsset.Id, TimeSpan.FromSeconds(10.1));
        observerMock.Verify(m => m.OnNext(true));
        await Task.Delay(TimeSpan.FromSeconds(15.1));
        assetRepositoryMock.Verify(m =>
            m.GetByIdAsync(dummyAsset.Id, It.IsAny<CancellationToken>())
        );
        remoteAssetServiceMock.Verify(m => m.FetchRemoteAssetsAsync(It.IsAny<CancellationToken>()));
        assetDirectoryServiceMock.VerifyNoOtherCalls();
        Assert.False(await service.IsPollingAsset(dummyAsset.Id).FirstAsync());
    }

    [Fact]
    public async Task StartPollNoDataTest()
    {
        AssetDb dummyAsset = new("SomePath.dummyBefore");
        byte[] dummyData = [1, 2, 3, 4, 5];
        const string dummyFileEnding = "dummyAfter";
        var assetRepositoryMock = new Mock<IAssetRepository>();
        assetRepositoryMock
            .Setup(m => m.GetByIdAsync(dummyAsset.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(dummyAsset));
        assetRepositoryMock
            .Setup(m => m.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        assetRepositoryMock
            .Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IReadOnlyCollection<AssetDb>>([dummyAsset]));
        var remoteAssetServiceMock = new Mock<IRemoteAssetService>();
        remoteAssetServiceMock
            .Setup(m => m.FetchRemoteAssetsAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        remoteAssetServiceMock
            .Setup(m => m.ReadRemoteAssetAsync(dummyAsset.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult((Data: Array.Empty<byte>(), FileEnding: dummyFileEnding)));
        remoteAssetServiceMock.Setup(m => m.AvailableAssets).Returns([dummyAsset.Id]);
        var assetDirectoryServiceMock = new Mock<IAssetDirectoryService>();
        assetDirectoryServiceMock
            .Setup(m =>
                m.WriteFileAsync(dummyAsset.RelativePath, dummyData, It.IsAny<CancellationToken>())
            )
            .Returns(Task.CompletedTask);
        assetDirectoryServiceMock
            .Setup(m => m.LocalServerPath(dummyAsset.RelativePath))
            .Returns(() => "SomeLocalServerPath");
        assetDirectoryServiceMock.Setup(m => m.Files).Returns(() => [dummyAsset.RelativePath]);
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(m => m.GetService(typeof(IAssetRepository)))
            .Returns(assetRepositoryMock.Object);
        var serviceScopeMock = new Mock<IServiceScope>();
        serviceScopeMock.Setup(m => m.ServiceProvider).Returns(serviceProviderMock.Object);
        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        serviceScopeFactoryMock.Setup(m => m.CreateScope()).Returns(serviceScopeMock.Object);
        var service = new PollAssetService(
            serviceScopeFactoryMock.Object,
            assetDirectoryServiceMock.Object,
            remoteAssetServiceMock.Object,
            new AssetStateService()
        );

        var observerMock = new Mock<IObserver<bool>>();
        service.IsPollingAsset(dummyAsset.Id).Subscribe(observerMock.Object);
        observerMock.Verify(m => m.OnNext(false));
        service.StartPollAsset(dummyAsset.Id, TimeSpan.FromSeconds(10.1));
        observerMock.Verify(m => m.OnNext(true));
        await Task.Delay(TimeSpan.FromSeconds(15.1));
        assetRepositoryMock.Verify(m =>
            m.GetByIdAsync(dummyAsset.Id, It.IsAny<CancellationToken>())
        );
        remoteAssetServiceMock.Verify(m => m.FetchRemoteAssetsAsync(It.IsAny<CancellationToken>()));
        remoteAssetServiceMock.Verify(m =>
            m.ReadRemoteAssetAsync(dummyAsset.Id, It.IsAny<CancellationToken>())
        );
        assetDirectoryServiceMock.VerifyNoOtherCalls();
        Assert.False(await service.IsPollingAsset(dummyAsset.Id).FirstAsync());
    }
}
