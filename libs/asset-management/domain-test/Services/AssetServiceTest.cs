using System.Reactive.Linq;
using System.Reactive.Subjects;
using MicraPro.AssetManagement.DataDefinition;
using MicraPro.AssetManagement.Domain.AssetAccess;
using MicraPro.AssetManagement.Domain.Interfaces;
using MicraPro.AssetManagement.Domain.Services;
using MicraPro.AssetManagement.Domain.StorageAccess;
using Microsoft.Extensions.Logging;
using Moq;

namespace MicraPro.AssetManagement.Domain.Test.Services;

public class AssetServiceTest
{
    private record DummyAsset(Guid Id, string Path) : IAsset
    {
        public bool IsAvailableLocally => true;
        public bool IsAvailableRemotely => true;
    }

    [Fact]
    public async Task RemoveAssetAsyncTest()
    {
        var asset = new AssetDb("MyPath");

        var assetRepositoryMock = new Mock<IAssetRepository>();
        assetRepositoryMock
            .Setup(m => m.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult(asset));
        assetRepositoryMock
            .Setup(m => m.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        assetRepositoryMock
            .Setup(m => m.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var assetDirectoryServiceMock = new Mock<IAssetDirectoryService>();
        assetDirectoryServiceMock
            .Setup(m => m.RemoveFileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var remoteAssetServiceMock = new Mock<IRemoteAssetService>();
        remoteAssetServiceMock
            .Setup(m => m.RemoveRemoteAssetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var pollAssetServiceMock = new Mock<IPollAssetService>();
        var service = new AssetService(
            assetRepositoryMock.Object,
            assetDirectoryServiceMock.Object,
            remoteAssetServiceMock.Object,
            new AssetStateService(),
            pollAssetServiceMock.Object,
            Mock.Of<ILogger<AssetService>>()
        );
        await service.RemoveAssetAsync(asset.Id, It.IsAny<CancellationToken>());
        assetRepositoryMock.Verify(m => m.DeleteAsync(asset.Id, It.IsAny<CancellationToken>()));
        assetRepositoryMock.Verify(m => m.SaveAsync(It.IsAny<CancellationToken>()));
        assetDirectoryServiceMock.Verify(m =>
            m.RemoveFileAsync("MyPath", It.IsAny<CancellationToken>())
        );
        remoteAssetServiceMock.Verify(m =>
            m.RemoveRemoteAssetAsync(asset.Id, It.IsAny<CancellationToken>())
        );
    }

    [Fact]
    public async Task SyncAssetsAsyncFailureTest()
    {
        var assetRepositoryMock = new Mock<IAssetRepository>();
        assetRepositoryMock
            .Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult<IReadOnlyCollection<AssetDb>>([]));
        var assetDirectoryServiceMock = new Mock<IAssetDirectoryService>();
        var remoteAssetServiceMock = new Mock<IRemoteAssetService>();
        remoteAssetServiceMock
            .Setup(m => m.FetchRemoteAssetsAsync(It.IsAny<CancellationToken>()))
            .Throws<Exception>();
        var pollAssetServiceMock = new Mock<IPollAssetService>();
        var service = new AssetService(
            assetRepositoryMock.Object,
            assetDirectoryServiceMock.Object,
            remoteAssetServiceMock.Object,
            new AssetStateService(),
            pollAssetServiceMock.Object,
            Mock.Of<ILogger<AssetService>>()
        );
        Assert.False(await service.SyncAssets(CancellationToken.None));
        assetRepositoryMock.Verify(m => m.GetAllAsync(It.IsAny<CancellationToken>()));
        remoteAssetServiceMock.Verify(m => m.FetchRemoteAssetsAsync(It.IsAny<CancellationToken>()));
        assetDirectoryServiceMock.VerifyNoOtherCalls();
        remoteAssetServiceMock.VerifyNoOtherCalls();
        assetRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SyncAssetsAsyncSuccessTest()
    {
        var assetData = (new byte[] { 1, 2, 3 }, "dummyEnding");
        // Available Locally and Remotely
        var asset1 = new AssetDb("MyPath1.someOtherEnding");
        // Available Remotely only
        var asset2 = new AssetDb("MyPath2.asset");
        // Available Remotely but not used
        var assetId3 = Guid.NewGuid();
        var assetRepositoryMock = new Mock<IAssetRepository>();
        assetRepositoryMock
            .Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult<IReadOnlyCollection<AssetDb>>([asset1, asset2]));
        assetRepositoryMock
            .Setup(m => m.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var assetDirectoryServiceMock = new Mock<IAssetDirectoryService>();
        assetDirectoryServiceMock
            .Setup(m => m.Files)
            .Returns([asset1.RelativePath, "SomeOtherFile"]);
        assetDirectoryServiceMock
            .Setup(m => m.RemoveFileAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        assetDirectoryServiceMock
            .Setup(m =>
                m.WriteFileAsync(
                    It.IsAny<string>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);
        var remoteAssetServiceMock = new Mock<IRemoteAssetService>();
        remoteAssetServiceMock
            .Setup(m => m.AvailableAssets)
            .Returns((IEnumerable<Guid>)[asset1.Id, asset2.Id, assetId3]);
        remoteAssetServiceMock
            .Setup(m => m.FetchRemoteAssetsAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        remoteAssetServiceMock
            .Setup(m => m.RemoveRemoteAssetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        remoteAssetServiceMock
            .Setup(m => m.ReadRemoteAssetAsync(asset2.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(assetData));

        var pollAssetServiceMock = new Mock<IPollAssetService>();
        var service = new AssetService(
            assetRepositoryMock.Object,
            assetDirectoryServiceMock.Object,
            remoteAssetServiceMock.Object,
            new AssetStateService(),
            pollAssetServiceMock.Object,
            Mock.Of<ILogger<AssetService>>()
        );
        Assert.True(await service.SyncAssets(CancellationToken.None));
        Assert.Equal("MyPath2.dummyEnding", asset2.RelativePath);
        assetRepositoryMock.Verify(m => m.SaveAsync(It.IsAny<CancellationToken>()));
        assetDirectoryServiceMock.Verify(m =>
            m.RemoveFileAsync("SomeOtherFile", It.IsAny<CancellationToken>())
        );
        assetDirectoryServiceMock.Verify(m =>
            m.WriteFileAsync("MyPath2.dummyEnding", assetData.Item1, It.IsAny<CancellationToken>())
        );
        assetDirectoryServiceMock.Verify(m => m.Files);
        remoteAssetServiceMock.Verify(m =>
            m.ReadRemoteAssetAsync(asset2.Id, It.IsAny<CancellationToken>())
        );
        remoteAssetServiceMock.Verify(m =>
            m.RemoveRemoteAssetAsync(assetId3, It.IsAny<CancellationToken>())
        );
        remoteAssetServiceMock.Verify(m => m.AvailableAssets);
        remoteAssetServiceMock.Verify(m => m.FetchRemoteAssetsAsync(It.IsAny<CancellationToken>()));
        assetDirectoryServiceMock.VerifyNoOtherCalls();
        remoteAssetServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateAssetAsyncTest()
    {
        var id = Guid.Empty;
        var assetRepositoryMock = new Mock<IAssetRepository>();
        assetRepositoryMock
            .Setup(m => m.AddAsync(It.IsAny<AssetDb>(), It.IsAny<CancellationToken>()))
            .Callback(
                (AssetDb a, CancellationToken _) =>
                {
                    id = a.Id;
                    Assert.Contains("MyFileName", a.RelativePath);
                }
            )
            .Returns(Task.CompletedTask);
        assetRepositoryMock
            .Setup(m => m.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var assetDirectoryServiceMock = new Mock<IAssetDirectoryService>();
        assetDirectoryServiceMock
            .Setup(m => m.CreateRandomFileNameWithoutExtension())
            .Returns("MyFileName");
        var remoteAssetServiceMock = new Mock<IRemoteAssetService>();
        remoteAssetServiceMock
            .Setup(m =>
                m.CreateAssetUploadPathAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())
            )
            .Returns(() => Task.FromResult("MyUploadPath"));
        var pollAssetServiceMock = new Mock<IPollAssetService>();
        var service = new AssetService(
            assetRepositoryMock.Object,
            assetDirectoryServiceMock.Object,
            remoteAssetServiceMock.Object,
            new AssetStateService(),
            pollAssetServiceMock.Object,
            Mock.Of<ILogger<AssetService>>()
        );
        var query = await service.CreateAssetAsync(CancellationToken.None);
        assetRepositoryMock.Verify(m =>
            m.AddAsync(It.IsAny<AssetDb>(), It.IsAny<CancellationToken>())
        );
        assetRepositoryMock.Verify(m => m.SaveAsync(It.IsAny<CancellationToken>()));
        assetDirectoryServiceMock.Verify(m => m.CreateRandomFileNameWithoutExtension());
        remoteAssetServiceMock.Verify(m =>
            m.CreateAssetUploadPathAsync(id, It.IsAny<CancellationToken>())
        );
        Assert.Equal(id, query.AssetId);
        Assert.Equal("MyUploadPath", query.UploadPath);
        assetDirectoryServiceMock.VerifyNoOtherCalls();
        remoteAssetServiceMock.VerifyNoOtherCalls();
        assetRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetAssetUploadQueryAsyncTest()
    {
        var id = Guid.NewGuid();
        var assetRepositoryMock = new Mock<IAssetRepository>();
        var assetDirectoryServiceMock = new Mock<IAssetDirectoryService>();
        var remoteAssetServiceMock = new Mock<IRemoteAssetService>();
        remoteAssetServiceMock
            .Setup(m =>
                m.CreateAssetUploadPathAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())
            )
            .Returns(() => Task.FromResult("MyUploadPath"));
        var pollAssetServiceMock = new Mock<IPollAssetService>();
        var service = new AssetService(
            assetRepositoryMock.Object,
            assetDirectoryServiceMock.Object,
            remoteAssetServiceMock.Object,
            new AssetStateService(),
            pollAssetServiceMock.Object,
            Mock.Of<ILogger<AssetService>>()
        );
        var query = await service.GetAssetUploadQueryAsync(id, CancellationToken.None);
        remoteAssetServiceMock.Verify(m =>
            m.CreateAssetUploadPathAsync(id, It.IsAny<CancellationToken>())
        );
        Assert.Equal(id, query.AssetId);
        Assert.Equal("MyUploadPath", query.UploadPath);
        assetDirectoryServiceMock.VerifyNoOtherCalls();
        remoteAssetServiceMock.VerifyNoOtherCalls();
        assetRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task PollAssetAsyncTest()
    {
        var id = Guid.NewGuid();
        var assetRepositoryMock = new Mock<IAssetRepository>();
        var assetDirectoryServiceMock = new Mock<IAssetDirectoryService>();
        var remoteAssetServiceMock = new Mock<IRemoteAssetService>();
        var pollAssetServiceMock = new Mock<IPollAssetService>();
        var service = new AssetService(
            assetRepositoryMock.Object,
            assetDirectoryServiceMock.Object,
            remoteAssetServiceMock.Object,
            new AssetStateService(),
            pollAssetServiceMock.Object,
            Mock.Of<ILogger<AssetService>>()
        );
        await service.PollAssetAsync(id, TimeSpan.Zero, CancellationToken.None);
        pollAssetServiceMock.Verify(m => m.StartPollAsset(id, TimeSpan.Zero));
        pollAssetServiceMock.VerifyNoOtherCalls();
        assetDirectoryServiceMock.VerifyNoOtherCalls();
        remoteAssetServiceMock.VerifyNoOtherCalls();
        assetRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task IsAssetPolling()
    {
        var id = Guid.NewGuid();
        var isPollingSubject = new BehaviorSubject<bool>(false);
        var assetRepositoryMock = new Mock<IAssetRepository>();
        var assetDirectoryServiceMock = new Mock<IAssetDirectoryService>();
        var remoteAssetServiceMock = new Mock<IRemoteAssetService>();
        var pollAssetServiceMock = new Mock<IPollAssetService>();
        pollAssetServiceMock.Setup(m => m.IsPollingAsset(id)).Returns(isPollingSubject);
        var service = new AssetService(
            assetRepositoryMock.Object,
            assetDirectoryServiceMock.Object,
            remoteAssetServiceMock.Object,
            new AssetStateService(),
            pollAssetServiceMock.Object,
            Mock.Of<ILogger<AssetService>>()
        );
        var isPolling = service.IsAssetPolling(id);
        pollAssetServiceMock.Verify(m => m.IsPollingAsset(id));
        Assert.False(await isPolling.FirstAsync());
        isPollingSubject.OnNext(true);
        Assert.True(await isPolling.FirstAsync());
        pollAssetServiceMock.VerifyNoOtherCalls();
        assetDirectoryServiceMock.VerifyNoOtherCalls();
        remoteAssetServiceMock.VerifyNoOtherCalls();
        assetRepositoryMock.VerifyNoOtherCalls();
    }
}
