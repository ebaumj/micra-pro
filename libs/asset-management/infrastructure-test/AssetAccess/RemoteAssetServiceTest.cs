using System.Text.Json;
using MicraPro.AssetManagement.Infrastructure.AssetAccess;
using MicraPro.AssetManagement.Infrastructure.Interfaces;
using MicraPro.AssetManagement.Infrastructure.ValueObjects;
using Microsoft.Extensions.Options;
using Moq;

namespace MicraPro.AssetManagement.Infrastructure.Test.AssetAccess;

public class RemoteAssetServiceTest
{
    [Fact]
    public async Task FetchRemoteAssetsAsyncTest()
    {
        var assetId1 = Guid.NewGuid();
        var assetId2 = Guid.NewGuid();
        const string token = "MyToken";
        var httpClientWrapperMock = new Mock<IHttpClientWrapper>();
        httpClientWrapperMock
            .Setup(m => m.MakeGetRequestAsync("MyDomain/api/assets", It.IsAny<CancellationToken>()))
            .Returns(
                Task.FromResult(
                    JsonSerializer.Serialize(
                        new AllAssetsPayload([assetId1.ToString(), assetId2.ToString()])
                    )
                )
            );
        var httpClientWrapperFactoryMock = new Mock<IHttpClientWrapperFactory>();
        httpClientWrapperFactoryMock
            .Setup(m => m.CreateClient(token))
            .Returns(httpClientWrapperMock.Object);
        var tokenCreatorServiceMock = new Mock<ITokenCreatorService>();
        tokenCreatorServiceMock.Setup(m => m.GenerateAccessToken()).Returns(token);
        var domainProviderMock = new Mock<IAssetServerDomainProvider>();
        domainProviderMock.Setup(m => m.AssetServerLocalDomain).Returns("MyDomain");
        var service = new RemoteAssetService(
            tokenCreatorServiceMock.Object,
            httpClientWrapperFactoryMock.Object,
            domainProviderMock.Object
        );
        Assert.Empty(service.AvailableAssets);
        await service.FetchRemoteAssetsAsync(CancellationToken.None);
        Assert.Contains(assetId1, service.AvailableAssets);
        Assert.Contains(assetId2, service.AvailableAssets);
        Assert.Equal(2, service.AvailableAssets.Count());
        tokenCreatorServiceMock.Verify(m => m.GenerateAccessToken(), Times.Once);
        httpClientWrapperFactoryMock.Verify(m => m.CreateClient(token), Times.Once);
        httpClientWrapperMock.Verify(
            m => m.MakeGetRequestAsync("MyDomain/api/assets", It.IsAny<CancellationToken>()),
            Times.Once
        );
        httpClientWrapperMock.Verify(m => m.Dispose());
        tokenCreatorServiceMock.VerifyNoOtherCalls();
        httpClientWrapperFactoryMock.VerifyNoOtherCalls();
        httpClientWrapperMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ReadRemoteAssetAsyncTest()
    {
        byte[] data = [1, 2, 3, 4, 5];
        var assetId = Guid.NewGuid();
        const string token = "MyToken";
        var endpoint = $"MyDomain/api/assets/{assetId.ToString()}";
        var httpClientWrapperMock = new Mock<IHttpClientWrapper>();
        httpClientWrapperMock
            .Setup(m => m.MakeGetRequestAsync(endpoint, It.IsAny<CancellationToken>()))
            .Returns(() =>
                Task.FromResult(
                    JsonSerializer.Serialize(
                        new AssetPayload(Convert.ToBase64String(data), "dummy")
                    )
                )
            );
        var httpClientWrapperFactoryMock = new Mock<IHttpClientWrapperFactory>();
        httpClientWrapperFactoryMock
            .Setup(m => m.CreateClient(token))
            .Returns(httpClientWrapperMock.Object);
        var tokenCreatorServiceMock = new Mock<ITokenCreatorService>();
        tokenCreatorServiceMock.Setup(m => m.GenerateAccessToken(assetId)).Returns(token);
        var domainProviderMock = new Mock<IAssetServerDomainProvider>();
        domainProviderMock.Setup(m => m.AssetServerLocalDomain).Returns("MyDomain");
        var service = new RemoteAssetService(
            tokenCreatorServiceMock.Object,
            httpClientWrapperFactoryMock.Object,
            domainProviderMock.Object
        );
        var result = await service.ReadRemoteAssetAsync(assetId, CancellationToken.None);
        Assert.Equal("dummy", result.FileEnding);
        Assert.Equivalent(data, result.Data);
        httpClientWrapperMock.Verify(
            m => m.MakeGetRequestAsync(endpoint, It.IsAny<CancellationToken>()),
            Times.Once
        );
        httpClientWrapperMock.Verify(m => m.Dispose(), Times.Once);
        tokenCreatorServiceMock.Verify(m => m.GenerateAccessToken(assetId), Times.Once);
        httpClientWrapperFactoryMock.Verify(m => m.CreateClient(token), Times.Once);
        tokenCreatorServiceMock.VerifyNoOtherCalls();
        httpClientWrapperFactoryMock.VerifyNoOtherCalls();
        httpClientWrapperMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RemoveRemoteAssetAsyncTest()
    {
        var assetId1 = Guid.NewGuid();
        var assetId2 = Guid.NewGuid();
        const string token = "MyToken";
        var endpoint = $"MyDomain/api/assets/{assetId1.ToString()}";
        var httpClientWrapperMock = new Mock<IHttpClientWrapper>();
        httpClientWrapperMock
            .Setup(m => m.MakeDeleteRequestAsync(endpoint, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        httpClientWrapperMock
            .Setup(m => m.MakeGetRequestAsync("MyDomain/api/assets", It.IsAny<CancellationToken>()))
            .Returns(
                Task.FromResult(
                    JsonSerializer.Serialize(
                        new AllAssetsPayload([assetId1.ToString(), assetId2.ToString()])
                    )
                )
            );
        var httpClientWrapperFactoryMock = new Mock<IHttpClientWrapperFactory>();
        httpClientWrapperFactoryMock
            .Setup(m => m.CreateClient(token))
            .Returns(httpClientWrapperMock.Object);
        var tokenCreatorServiceMock = new Mock<ITokenCreatorService>();
        tokenCreatorServiceMock.Setup(m => m.GenerateAccessToken(assetId1)).Returns(token);
        tokenCreatorServiceMock.Setup(m => m.GenerateAccessToken()).Returns(token);
        var domainProviderMock = new Mock<IAssetServerDomainProvider>();
        domainProviderMock.Setup(m => m.AssetServerLocalDomain).Returns("MyDomain");
        var service = new RemoteAssetService(
            tokenCreatorServiceMock.Object,
            httpClientWrapperFactoryMock.Object,
            domainProviderMock.Object
        );
        Assert.Empty(service.AvailableAssets);
        await service.FetchRemoteAssetsAsync(CancellationToken.None);
        Assert.Contains(assetId1, service.AvailableAssets);
        Assert.Contains(assetId2, service.AvailableAssets);
        Assert.Equal(2, service.AvailableAssets.Count());
        tokenCreatorServiceMock.Verify(m => m.GenerateAccessToken(), Times.Once);
        httpClientWrapperFactoryMock.Verify(m => m.CreateClient(token), Times.Once);
        httpClientWrapperMock.Verify(
            m => m.MakeGetRequestAsync("MyDomain/api/assets", It.IsAny<CancellationToken>()),
            Times.Once
        );
        httpClientWrapperMock.Verify(m => m.Dispose());
        await service.RemoveRemoteAssetAsync(assetId1, CancellationToken.None);
        Assert.DoesNotContain(assetId1, service.AvailableAssets);
        Assert.Contains(assetId2, service.AvailableAssets);
        Assert.Single(service.AvailableAssets);
        httpClientWrapperMock.Verify(
            m => m.MakeDeleteRequestAsync(endpoint, It.IsAny<CancellationToken>()),
            Times.Once
        );
        httpClientWrapperMock.Verify(m => m.Dispose(), Times.Exactly(2));
        tokenCreatorServiceMock.Verify(m => m.GenerateAccessToken(assetId1), Times.Once);
        httpClientWrapperFactoryMock.Verify(m => m.CreateClient(token), Times.Exactly(2));
        tokenCreatorServiceMock.VerifyNoOtherCalls();
        httpClientWrapperFactoryMock.VerifyNoOtherCalls();
        httpClientWrapperMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CreateAssetUploadPathAsync()
    {
        var assetId1 = Guid.NewGuid();
        var assetId2 = Guid.NewGuid();
        const string token1 = "MyToken1";
        const string token2 = "MyToken2";
        var tokenCreatorServiceMock = new Mock<ITokenCreatorService>();
        tokenCreatorServiceMock.Setup(m => m.GenerateUploadAccessToken(assetId1)).Returns(token1);
        tokenCreatorServiceMock.Setup(m => m.GenerateUploadAccessToken(assetId2)).Returns(token2);
        var domainProviderMock = new Mock<IAssetServerDomainProvider>();
        domainProviderMock.Setup(m => m.AssetServerExternDomain).Returns("MyDomain");
        var service = new RemoteAssetService(
            tokenCreatorServiceMock.Object,
            Mock.Of<IHttpClientWrapperFactory>(),
            domainProviderMock.Object
        );
        Assert.Equal(
            $"MyDomain/upload/{assetId1.ToString()}?token=MyToken1",
            await service.CreateAssetUploadPathAsync(assetId1, CancellationToken.None)
        );
        Assert.Equal(
            $"MyDomain/upload/{assetId2.ToString()}?token=MyToken2",
            await service.CreateAssetUploadPathAsync(assetId2, CancellationToken.None)
        );
    }
}
