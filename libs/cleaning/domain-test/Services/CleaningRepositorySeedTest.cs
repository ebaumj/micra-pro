using MicraPro.Cleaning.DataDefinition.ValueObjects;
using MicraPro.Cleaning.Domain.Interfaces;
using MicraPro.Cleaning.Domain.StorageAccess;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace MicraPro.Cleaning.Domain.Test.Services;

public class CleaningRepositorySeedTest
{
    [Fact]
    public async Task StartAsyncNoDataTest()
    {
        CleaningCycle[] data = [];
        var repositoryMock = new Mock<ICleaningRepository>();
        repositoryMock
            .Setup(m => m.SetCleaningSequenceAsync(data, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        repositoryMock
            .Setup(m => m.IsCleaningSetAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(false));
        var service = new CleaningRepositorySeed(
            Mock.Of<IServiceScopeFactory>(scp =>
                scp.CreateScope()
                == Mock.Of<IServiceScope>(sc =>
                    sc.ServiceProvider
                    == Mock.Of<IServiceProvider>(sp =>
                        sp.GetService(typeof(ICleaningRepository)) == repositoryMock.Object
                    )
                )
            ),
            Mock.Of<ICleaningDefaultsProvider>(m => m.DefaultSequence == data)
        );
        await service.StartAsync(CancellationToken.None);
        repositoryMock.Verify(
            m => m.SetCleaningSequenceAsync(data, It.IsAny<CancellationToken>()),
            Times.Once
        );
        repositoryMock.Verify(m => m.IsCleaningSetAsync(It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task StartAsyncDataTest()
    {
        CleaningCycle[] data = [];
        var repositoryMock = new Mock<ICleaningRepository>();
        repositoryMock
            .Setup(m => m.SetCleaningSequenceAsync(data, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        repositoryMock
            .Setup(m => m.IsCleaningSetAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(true));
        var service = new CleaningRepositorySeed(
            Mock.Of<IServiceScopeFactory>(scp =>
                scp.CreateScope()
                == Mock.Of<IServiceScope>(sc =>
                    sc.ServiceProvider
                    == Mock.Of<IServiceProvider>(sp =>
                        sp.GetService(typeof(ICleaningRepository)) == repositoryMock.Object
                    )
                )
            ),
            Mock.Of<ICleaningDefaultsProvider>(m => m.DefaultSequence == data)
        );
        await service.StartAsync(CancellationToken.None);
        repositoryMock.Verify(m => m.IsCleaningSetAsync(It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
    }
}
