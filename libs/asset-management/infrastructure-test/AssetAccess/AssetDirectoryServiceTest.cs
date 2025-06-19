using MicraPro.AssetManagement.Infrastructure.AssetAccess;
using MicraPro.AssetManagement.Infrastructure.Interfaces;
using Microsoft.Extensions.Options;
using Moq;

namespace MicraPro.AssetManagement.Infrastructure.Test.AssetAccess;

public class AssetDirectoryServiceTest
{
    private class Options(string localFileServerDomain, string localFileServerFolder)
        : IOptions<AssetManagementInfrastructureOptions>
    {
        public AssetManagementInfrastructureOptions Value { get; } =
            new()
            {
                LocalFileServerDomain = localFileServerDomain,
                LocalFileServerFolder = localFileServerFolder,
            };
    }

    [Fact]
    public void LocalServerPathTest()
    {
        var service = new AssetDirectoryService(
            Mock.Of<IFileSystemAccess>(),
            new Options("MyDomain", "MyFolder")
        );
        Assert.Equal("MyDomain/MyFile", service.LocalServerPath("MyFile"));
    }

    [Fact]
    public async Task ReadFilesDirectoryExistsAsyncTest()
    {
        var fileSystemAccessMock = new Mock<IFileSystemAccess>();
        fileSystemAccessMock.Setup(m => m.DirectoryExists("MyFolder")).Returns(true);
        fileSystemAccessMock
            .Setup(m => m.GetFiles("MyFolder", "*.*", SearchOption.AllDirectories))
            .Returns([Path.Combine("MyFolder", "File1"), Path.Combine("MyFolder", "File2")]);
        var service = new AssetDirectoryService(
            fileSystemAccessMock.Object,
            new Options("MyDomain", "MyFolder")
        );
        Assert.Empty(service.Files);
        await service.ReadFilesAsync(CancellationToken.None);
        Assert.Contains("File1", service.Files);
        Assert.Contains("File2", service.Files);
        Assert.Equal(2, service.Files.Count());
        fileSystemAccessMock.Verify(m => m.DirectoryExists("MyFolder"), Times.Once);
        fileSystemAccessMock.Verify(
            m => m.GetFiles("MyFolder", "*.*", SearchOption.AllDirectories),
            Times.Once
        );
        fileSystemAccessMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ReadFilesDirectoryDoesNotExistAsyncTest()
    {
        var fileSystemAccessMock = new Mock<IFileSystemAccess>();
        fileSystemAccessMock.Setup(m => m.DirectoryExists("MyFolder")).Returns(false);
        fileSystemAccessMock
            .Setup(m => m.GetFiles("MyFolder", "*.*", SearchOption.AllDirectories))
            .Returns([Path.Combine("MyFolder", "File1"), Path.Combine("MyFolder", "File2")]);
        var service = new AssetDirectoryService(
            fileSystemAccessMock.Object,
            new Options("MyDomain", "MyFolder")
        );
        Assert.Empty(service.Files);
        await service.ReadFilesAsync(CancellationToken.None);
        Assert.Contains("File1", service.Files);
        Assert.Contains("File2", service.Files);
        Assert.Equal(2, service.Files.Count());
        fileSystemAccessMock.Verify(m => m.DirectoryExists("MyFolder"), Times.Once);
        fileSystemAccessMock.Verify(m => m.CreateDirectory("MyFolder"), Times.Once);
        fileSystemAccessMock.Verify(
            m => m.GetFiles("MyFolder", "*.*", SearchOption.AllDirectories),
            Times.Once
        );
        fileSystemAccessMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task WriteFileDirectoryExistsAsyncTest()
    {
        byte[] data = [1, 2, 3];
        var fileSystemAccessMock = new Mock<IFileSystemAccess>();
        fileSystemAccessMock.Setup(m => m.DirectoryExists("MyFolder")).Returns(true);
        fileSystemAccessMock
            .Setup(m =>
                m.WriteFileAsync(
                    It.IsAny<string>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);
        var service = new AssetDirectoryService(
            fileSystemAccessMock.Object,
            new Options("MyDomain", "MyFolder")
        );
        Assert.Empty(service.Files);
        await service.WriteFileAsync("MyFile", data, CancellationToken.None);
        Assert.Contains("MyFile", service.Files);
        Assert.Single(service.Files);
        fileSystemAccessMock.Verify(m => m.DirectoryExists("MyFolder"), Times.Once);
        fileSystemAccessMock.Verify(
            m => m.WriteFileAsync(Path.Join("MyFolder", "MyFile"), data, CancellationToken.None),
            Times.Once
        );
        fileSystemAccessMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task WriteFileDirectoryDoesNotExistAsyncTest()
    {
        byte[] data = [1, 2, 3];
        var fileSystemAccessMock = new Mock<IFileSystemAccess>();
        fileSystemAccessMock.Setup(m => m.DirectoryExists("MyFolder")).Returns(false);
        fileSystemAccessMock
            .Setup(m =>
                m.WriteFileAsync(
                    It.IsAny<string>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);
        var service = new AssetDirectoryService(
            fileSystemAccessMock.Object,
            new Options("MyDomain", "MyFolder")
        );
        Assert.Empty(service.Files);
        await service.WriteFileAsync("MyFile", data, CancellationToken.None);
        Assert.Contains("MyFile", service.Files);
        Assert.Single(service.Files);
        fileSystemAccessMock.Verify(m => m.DirectoryExists("MyFolder"), Times.Once);
        fileSystemAccessMock.Verify(m => m.CreateDirectory("MyFolder"), Times.Once);
        fileSystemAccessMock.Verify(
            m => m.WriteFileAsync(Path.Join("MyFolder", "MyFile"), data, CancellationToken.None),
            Times.Once
        );
        fileSystemAccessMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RemoveFileAsyncTest()
    {
        var fileSystemAccessMock = new Mock<IFileSystemAccess>();
        fileSystemAccessMock.Setup(m => m.DirectoryExists("MyFolder")).Returns(true);
        fileSystemAccessMock
            .Setup(m => m.GetFiles("MyFolder", "*.*", SearchOption.AllDirectories))
            .Returns([Path.Combine("MyFolder", "File1"), Path.Combine("MyFolder", "File2")]);
        var service = new AssetDirectoryService(
            fileSystemAccessMock.Object,
            new Options("MyDomain", "MyFolder")
        );
        ;
        Assert.Empty(service.Files);
        await service.ReadFilesAsync(CancellationToken.None);
        fileSystemAccessMock.Verify(m => m.DirectoryExists("MyFolder"), Times.Once);
        fileSystemAccessMock.Verify(
            m => m.GetFiles("MyFolder", "*.*", SearchOption.AllDirectories),
            Times.Once
        );
        Assert.Contains("File1", service.Files);
        Assert.Contains("File2", service.Files);
        Assert.Equal(2, service.Files.Count());
        await service.RemoveFileAsync("File1", CancellationToken.None);
        fileSystemAccessMock.Verify(m => m.DeleteFile(Path.Join("MyFolder", "File1")), Times.Once);
        Assert.DoesNotContain("File1", service.Files);
        Assert.Contains("File2", service.Files);
        Assert.Single(service.Files);
    }
}
