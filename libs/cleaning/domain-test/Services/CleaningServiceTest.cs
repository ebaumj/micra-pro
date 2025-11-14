using MicraPro.Cleaning.DataDefinition.ValueObjects;
using MicraPro.Cleaning.Domain.HardwareAccess;
using MicraPro.Cleaning.Domain.Interfaces;
using MicraPro.Cleaning.Domain.Services;
using MicraPro.Cleaning.Domain.StorageAccess;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace MicraPro.Cleaning.Domain.Test.Services;

public class CleaningServiceTest
{
    private static CancellationToken AnyCt => It.IsAny<CancellationToken>();

    private static IServiceScopeFactory Sf(ICleaningRepository r) =>
        Mock.Of<IServiceScopeFactory>(sf =>
            sf.CreateScope()
            == Mock.Of<IServiceScope>(s =>
                s.ServiceProvider
                == Mock.Of<IServiceProvider>(sp => sp.GetService(typeof(ICleaningRepository)) == r)
            )
        );

    [Fact]
    public async Task GetCleaningSequenceAsyncTest()
    {
        CleaningCycle[] data = [];
        var repositoryMock = new Mock<ICleaningRepository>();
        repositoryMock.Setup(m => m.GetCleaningSequenceAsync(AnyCt)).ReturnsAsync(data);
        var service = new CleaningService(
            repositoryMock.Object,
            Mock.Of<IBrewPaddle>(),
            Mock.Of<ICleaningStateService>(),
            Mock.Of<ICleaningDefaultsProvider>(),
            Sf(repositoryMock.Object)
        );
        Assert.Equal(await service.GetCleaningSequenceAsync(CancellationToken.None), data);
        repositoryMock.Verify(m => m.GetCleaningSequenceAsync(AnyCt), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SetCleaningSequenceAsyncTest()
    {
        CleaningCycle[] data = [];
        var repositoryMock = new Mock<ICleaningRepository>();
        repositoryMock
            .Setup(m => m.SetCleaningSequenceAsync(data, AnyCt))
            .Returns(Task.CompletedTask);
        var service = new CleaningService(
            repositoryMock.Object,
            Mock.Of<IBrewPaddle>(),
            Mock.Of<ICleaningStateService>(),
            Mock.Of<ICleaningDefaultsProvider>(),
            Sf(repositoryMock.Object)
        );
        await service.SetCleaningSequenceAsync(data, CancellationToken.None);
        repositoryMock.Verify(m => m.SetCleaningSequenceAsync(data, AnyCt), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ResetCleaningSequenceAsyncTest()
    {
        CleaningCycle[] data = [];
        var repositoryMock = new Mock<ICleaningRepository>();
        repositoryMock
            .Setup(m => m.SetCleaningSequenceAsync(data, AnyCt))
            .Returns(Task.CompletedTask);
        var service = new CleaningService(
            repositoryMock.Object,
            Mock.Of<IBrewPaddle>(),
            Mock.Of<ICleaningStateService>(),
            Mock.Of<ICleaningDefaultsProvider>(m => m.DefaultSequence == data),
            Sf(repositoryMock.Object)
        );
        await service.ResetCleaningSequenceAsync(CancellationToken.None);
        repositoryMock.Verify(m => m.SetCleaningSequenceAsync(data, AnyCt), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetCleaningIntervalAsyncTest()
    {
        var data = TimeSpan.FromMinutes(2.1);
        var repositoryMock = new Mock<ICleaningRepository>();
        repositoryMock.Setup(m => m.GetCleaningIntervalAsync(AnyCt)).ReturnsAsync(data);
        var service = new CleaningService(
            repositoryMock.Object,
            Mock.Of<IBrewPaddle>(),
            Mock.Of<ICleaningStateService>(),
            Mock.Of<ICleaningDefaultsProvider>(),
            Sf(repositoryMock.Object)
        );
        Assert.Equal(await service.GetCleaningIntervalAsync(CancellationToken.None), data);
        repositoryMock.Verify(m => m.GetCleaningIntervalAsync(AnyCt), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task SetCleaningIntervalAsyncTest()
    {
        var data = TimeSpan.FromMinutes(2.1);
        var repositoryMock = new Mock<ICleaningRepository>();
        repositoryMock
            .Setup(m => m.SetCleaningIntervalAsync(data, AnyCt))
            .Returns(Task.CompletedTask);
        var service = new CleaningService(
            repositoryMock.Object,
            Mock.Of<IBrewPaddle>(),
            Mock.Of<ICleaningStateService>(),
            Mock.Of<ICleaningDefaultsProvider>(),
            Sf(repositoryMock.Object)
        );
        await service.SetCleaningIntervalAsync(data, CancellationToken.None);
        repositoryMock.Verify(m => m.SetCleaningIntervalAsync(data, AnyCt), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetLastCleaningTimeAsyncTest()
    {
        var data = DateTime.Now;
        var repositoryMock = new Mock<ICleaningRepository>();
        repositoryMock.Setup(m => m.GetLastCleaningTimeAsync(AnyCt)).ReturnsAsync(data);
        var service = new CleaningService(
            repositoryMock.Object,
            Mock.Of<IBrewPaddle>(),
            Mock.Of<ICleaningStateService>(),
            Mock.Of<ICleaningDefaultsProvider>(),
            Sf(repositoryMock.Object)
        );
        Assert.Equal(await service.GetLastCleaningTimeAsync(CancellationToken.None), data);
        repositoryMock.Verify(m => m.GetLastCleaningTimeAsync(AnyCt), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task StartCleaningSuccessTest()
    {
        CleaningCycle[] data =
        [
            new(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(100)),
            new(TimeSpan.FromMilliseconds(150), TimeSpan.FromMilliseconds(50)),
        ];
        var repositoryMock = new Mock<ICleaningRepository>();
        repositoryMock.Setup(m => m.GetCleaningSequenceAsync(AnyCt)).ReturnsAsync(data);
        repositoryMock
            .Setup(m => m.SetLastCleaningTimeAsync(It.IsAny<DateTime>(), AnyCt))
            .Returns(Task.CompletedTask);
        var brewPaddleMock = new Mock<IBrewPaddle>();
        brewPaddleMock
            .Setup(m => m.SetPaddleAsync(It.IsAny<bool>(), AnyCt))
            .Returns(Task.CompletedTask);
        var observerMock = new Mock<IObserver<CleaningState>>();
        var runningObserverMock = new Mock<IObserver<bool>>();
        var service = new CleaningService(
            repositoryMock.Object,
            brewPaddleMock.Object,
            new CleaningStateService(),
            Mock.Of<ICleaningDefaultsProvider>(),
            Sf(repositoryMock.Object)
        );
        service.IsRunning.Subscribe(runningObserverMock.Object);
        runningObserverMock.Verify(m => m.OnNext(false), Times.Once);
        service.StartCleaning(CancellationToken.None).Subscribe(observerMock.Object);
        runningObserverMock.Verify(m => m.OnNext(true), Times.Once);
        observerMock.Verify(m => m.OnNext(It.IsAny<CleaningState.Started>()), Times.Once);
        await Task.Delay(TimeSpan.FromMilliseconds(20));
        repositoryMock.Verify(
            m => m.GetCleaningSequenceAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
        brewPaddleMock.Verify(m => m.SetPaddleAsync(true, AnyCt), Times.Once);
        await Task.Delay(TimeSpan.FromMilliseconds(50));
        brewPaddleMock.Verify(m => m.SetPaddleAsync(false, AnyCt), Times.Once);
        await Task.Delay(TimeSpan.FromMilliseconds(150));
        brewPaddleMock.Verify(m => m.SetPaddleAsync(true, AnyCt), Times.Exactly(2));
        await Task.Delay(TimeSpan.FromMilliseconds(100));
        observerMock.Verify(
            m =>
                m.OnNext(
                    It.Is<CleaningState.Running>(r =>
                        r.CycleNumber == 1
                        && r.TotalTime > TimeSpan.FromMilliseconds(160)
                        && r.CycleTime > TimeSpan.FromMilliseconds(10)
                    )
                ),
            Times.Once
        );
        await Task.Delay(TimeSpan.FromMilliseconds(50));
        brewPaddleMock.Verify(m => m.SetPaddleAsync(false, AnyCt), Times.Exactly(3));
        await Task.Delay(TimeSpan.FromMilliseconds(50));
        observerMock.Verify(
            m =>
                m.OnNext(
                    It.Is<CleaningState.Finished>(r =>
                        r.TotalTime > TimeSpan.FromMilliseconds(350) && r.TotalCycles == 2
                    )
                ),
            Times.Once
        );
        observerMock.Verify(m => m.OnCompleted(), Times.Once);
        runningObserverMock.Verify(m => m.OnNext(false), Times.Exactly(2));
        repositoryMock.Verify(m => m.SetLastCleaningTimeAsync(It.IsAny<DateTime>(), AnyCt));
        observerMock.VerifyNoOtherCalls();
        runningObserverMock.VerifyNoOtherCalls();
        brewPaddleMock.VerifyNoOtherCalls();
        repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task StartCleaningCancelTest()
    {
        CleaningCycle[] data =
        [
            new(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(50)),
            new(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(50)),
        ];
        var ct = new CancellationTokenSource();
        var repositoryMock = new Mock<ICleaningRepository>();
        repositoryMock.Setup(m => m.GetCleaningSequenceAsync(AnyCt)).ReturnsAsync(data);
        repositoryMock
            .Setup(m => m.SetLastCleaningTimeAsync(It.IsAny<DateTime>(), AnyCt))
            .Returns(Task.CompletedTask);
        var brewPaddleMock = new Mock<IBrewPaddle>();
        brewPaddleMock
            .Setup(m => m.SetPaddleAsync(It.IsAny<bool>(), AnyCt))
            .Returns(Task.CompletedTask);
        var observerMock = new Mock<IObserver<CleaningState>>();
        var runningObserverMock = new Mock<IObserver<bool>>();
        var service = new CleaningService(
            repositoryMock.Object,
            brewPaddleMock.Object,
            new CleaningStateService(),
            Mock.Of<ICleaningDefaultsProvider>(),
            Sf(repositoryMock.Object)
        );
        service.IsRunning.Subscribe(runningObserverMock.Object);
        runningObserverMock.Verify(m => m.OnNext(false), Times.Once);
        service.StartCleaning(ct.Token).Subscribe(observerMock.Object);
        runningObserverMock.Verify(m => m.OnNext(true), Times.Once);
        observerMock.Verify(m => m.OnNext(It.IsAny<CleaningState.Started>()), Times.Once);
        await Task.Delay(TimeSpan.FromMilliseconds(20), CancellationToken.None);
        repositoryMock.Verify(
            m => m.GetCleaningSequenceAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
        brewPaddleMock.Verify(m => m.SetPaddleAsync(true, AnyCt), Times.Once);
        await Task.Delay(TimeSpan.FromMilliseconds(50), CancellationToken.None);
        brewPaddleMock.Verify(m => m.SetPaddleAsync(false, AnyCt), Times.Once);
        await Task.Delay(TimeSpan.FromMilliseconds(50), CancellationToken.None);
        brewPaddleMock.Verify(m => m.SetPaddleAsync(true, AnyCt), Times.Exactly(2));
        await ct.CancelAsync();
        await Task.Delay(TimeSpan.FromMilliseconds(20), CancellationToken.None);
        observerMock.Verify(
            m =>
                m.OnNext(
                    It.Is<CleaningState.Cancelled>(c =>
                        c.TotalTime > TimeSpan.FromMilliseconds(110) && c.TotalCycles == 1
                    )
                ),
            Times.Once
        );
        observerMock.Verify(m => m.OnCompleted(), Times.Once);
        brewPaddleMock.Verify(m => m.SetPaddleAsync(false, AnyCt), Times.Exactly(2));
        runningObserverMock.Verify(m => m.OnNext(false), Times.Exactly(2));
        observerMock.VerifyNoOtherCalls();
        runningObserverMock.VerifyNoOtherCalls();
        brewPaddleMock.VerifyNoOtherCalls();
        repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task StartCleaningFailTest()
    {
        CleaningCycle[] data =
        [
            new(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(50)),
            new(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(50)),
        ];
        var repositoryMock = new Mock<ICleaningRepository>();
        repositoryMock.Setup(m => m.GetCleaningSequenceAsync(AnyCt)).ReturnsAsync(data);
        repositoryMock
            .Setup(m => m.SetLastCleaningTimeAsync(It.IsAny<DateTime>(), AnyCt))
            .Returns(Task.CompletedTask);
        var brewPaddleMock = new Mock<IBrewPaddle>();
        brewPaddleMock
            .Setup(m => m.SetPaddleAsync(It.IsAny<bool>(), AnyCt))
            .Throws(new Exception());
        var observerMock = new Mock<IObserver<CleaningState>>();
        var runningObserverMock = new Mock<IObserver<bool>>();
        var service = new CleaningService(
            repositoryMock.Object,
            brewPaddleMock.Object,
            new CleaningStateService(),
            Mock.Of<ICleaningDefaultsProvider>(),
            Sf(repositoryMock.Object)
        );
        service.IsRunning.Subscribe(runningObserverMock.Object);
        runningObserverMock.Verify(m => m.OnNext(false), Times.Once);
        service.StartCleaning(CancellationToken.None).Subscribe(observerMock.Object);
        runningObserverMock.Verify(m => m.OnNext(true), Times.Once);
        observerMock.Verify(m => m.OnNext(It.IsAny<CleaningState.Started>()), Times.Once);
        await Task.Delay(TimeSpan.FromMilliseconds(20), CancellationToken.None);
        observerMock.Verify(
            m => m.OnNext(It.Is<CleaningState.Failed>(c => c.TotalCycles == 0)),
            Times.Once
        );
        observerMock.Verify(m => m.OnCompleted(), Times.Once);
        repositoryMock.Verify(
            m => m.GetCleaningSequenceAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
        brewPaddleMock.Verify(m => m.SetPaddleAsync(true, AnyCt), Times.Once);
        brewPaddleMock.Verify(m => m.SetPaddleAsync(false, AnyCt), Times.Once);
        runningObserverMock.Verify(m => m.OnNext(false), Times.Exactly(2));
        observerMock.VerifyNoOtherCalls();
        runningObserverMock.VerifyNoOtherCalls();
        brewPaddleMock.VerifyNoOtherCalls();
        repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void StartCleaningAlreadyRunningTest()
    {
        var cleaningService = new CleaningStateService();
        var service = new CleaningService(
            Mock.Of<ICleaningRepository>(),
            Mock.Of<IBrewPaddle>(),
            cleaningService,
            Mock.Of<ICleaningDefaultsProvider>(),
            Sf(Mock.Of<ICleaningRepository>())
        );
        cleaningService.SetIsRunning(true);
        Assert.Throws<Exception>(() => service.StartCleaning(CancellationToken.None));
    }
}
