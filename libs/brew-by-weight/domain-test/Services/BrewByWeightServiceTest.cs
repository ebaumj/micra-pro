using System.Reactive.Linq;
using System.Reactive.Subjects;
using MicraPro.BrewByWeight.DataDefinition;
using MicraPro.BrewByWeight.DataDefinition.ValueObjects;
using MicraPro.BrewByWeight.Domain.HardwareAccess;
using MicraPro.BrewByWeight.Domain.Interfaces;
using MicraPro.BrewByWeight.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace MicraPro.BrewByWeight.Domain.Test.Services;

public class BrewByWeightServiceTest
{
    private Mock<IPaddleAccess> CreatePaddleAccessMock()
    {
        var mock = new Mock<IPaddleAccess>();
        mock.Setup(m => m.SetBrewPaddleOnAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        return mock;
    }

    private Mock<IBrewByWeightDbService> CreateBrewByWeightDbServiceMock()
    {
        var mock = new Mock<IBrewByWeightDbService>();
        mock.Setup(m =>
                m.StoreProcessAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<IBrewByWeightService.Spout>(),
                    It.IsAny<IReadOnlyCollection<BrewByWeightTracking>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.CompletedTask);
        return mock;
    }

    private Mock<IScaleConnection> CreateScaleConnectionMock(IObservable<ScaleDataPoint> dataPoints)
    {
        var mock = new Mock<IScaleConnection>();
        mock.Setup(m => m.Data).Returns(dataPoints);
        mock.Setup(m => m.TareAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        mock.Setup(m => m.DisconnectAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        return mock;
    }

    private BrewByWeightService CreateService(
        double retention,
        IPaddleAccess paddleAccess,
        IScaleConnection scaleConnection,
        IBrewByWeightDbService brewByWeightDbService,
        bool scaleThrowException = false
    )
    {
        var retentionServiceMock = new Mock<IRetentionService>();
        retentionServiceMock
            .Setup(m =>
                m.CalculateRetentionWeightAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<IBrewByWeightService.Spout>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.FromResult(retention));
        var scaleAccessMock = new Mock<IScaleAccess>();
        if (scaleThrowException)
            scaleAccessMock
                .Setup(m => m.ConnectScaleAsync(It.IsAny<CancellationToken>()))
                .Throws(new Exception());
        else
            scaleAccessMock
                .Setup(m => m.ConnectScaleAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(scaleConnection));
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(m => m.GetService(typeof(IBrewByWeightDbService)))
            .Returns(brewByWeightDbService);
        var serviceScopeMock = new Mock<IServiceScope>();
        serviceScopeMock.Setup(m => m.ServiceProvider).Returns(serviceProviderMock.Object);
        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        serviceScopeFactoryMock.Setup(m => m.CreateScope()).Returns(serviceScopeMock.Object);
        return new BrewByWeightService(
            retentionServiceMock.Object,
            paddleAccess,
            scaleAccessMock.Object,
            serviceScopeFactoryMock.Object,
            Mock.Of<ILogger<BrewByWeightService>>()
        );
    }

    [Theory]
    [InlineData(IBrewByWeightService.Spout.Single)]
    [InlineData(IBrewByWeightService.Spout.Naked)]
    public async Task RunBrewByWeightSingleSpoutSuccessTest(IBrewByWeightService.Spout spout)
    {
        var stateObserverMock = new Mock<IObserver<BrewByWeightState>>();
        var trackingObserverMock = new Mock<IObserver<BrewByWeightTracking>>();
        var paddleAccessMock = CreatePaddleAccessMock();
        var dbServiceMock = CreateBrewByWeightDbServiceMock();
        var scaleDataSubject = new Subject<ScaleDataPoint>();
        var scaleMock = CreateScaleConnectionMock(scaleDataSubject);
        var service = CreateService(
            3,
            paddleAccessMock.Object,
            scaleMock.Object,
            dbServiceMock.Object
        );
        service.State.Subscribe(stateObserverMock.Object);
        stateObserverMock.Verify(m => m.OnNext(It.IsAny<BrewByWeightState.Idle>()), Times.Once);
        var process = service.RunBrewByWeight(Guid.NewGuid(), 40, 1, 1, TimeSpan.Zero, spout);
        stateObserverMock.Verify(
            m => m.OnNext(new BrewByWeightState.Running(process.ProcessId)),
            Times.Once
        );
        process.State.Subscribe(trackingObserverMock.Object);
        trackingObserverMock.Verify(m => m.OnNext(new BrewByWeightTracking.Started()), Times.Once);
        scaleMock.Verify(m => m.TareAsync(It.IsAny<CancellationToken>()), Times.Once);
        paddleAccessMock.Verify(
            m => m.SetBrewPaddleOnAsync(true, It.IsAny<CancellationToken>()),
            Times.Never
        );
        scaleDataSubject.OnNext(new ScaleDataPoint(0, 0));
        paddleAccessMock.Verify(
            m => m.SetBrewPaddleOnAsync(true, It.IsAny<CancellationToken>()),
            Times.Once
        );
        paddleAccessMock.Verify(
            m => m.SetBrewPaddleOnAsync(false, It.IsAny<CancellationToken>()),
            Times.Never
        );
        scaleDataSubject.OnNext(new ScaleDataPoint(10, 1));
        paddleAccessMock.Verify(
            m => m.SetBrewPaddleOnAsync(false, It.IsAny<CancellationToken>()),
            Times.Never
        );
        scaleDataSubject.OnNext(new ScaleDataPoint(36.9, 1));
        paddleAccessMock.Verify(
            m => m.SetBrewPaddleOnAsync(false, It.IsAny<CancellationToken>()),
            Times.Never
        );
        scaleDataSubject.OnNext(new ScaleDataPoint(37, 1));
        paddleAccessMock.Verify(
            m => m.SetBrewPaddleOnAsync(false, It.IsAny<CancellationToken>()),
            Times.Once
        );
        scaleDataSubject.OnNext(new ScaleDataPoint(40, 0));
        await Task.Delay(TimeSpan.FromSeconds(3.1));
        trackingObserverMock.Verify(
            m => m.OnNext(It.IsAny<BrewByWeightTracking.Finished>()),
            Times.Once
        );
        trackingObserverMock.Verify(m => m.OnCompleted(), Times.Once);
        stateObserverMock.Verify(
            m => m.OnNext(It.IsAny<BrewByWeightState.Idle>()),
            Times.Exactly(2)
        );
        Assert.True(
            await process
                .State.OfType<BrewByWeightTracking.Running>()
                .Select(s => s.Flow)
                .SequenceEqual([1, 1, 1, 0])
        );
        Assert.True(
            await process
                .State.OfType<BrewByWeightTracking.Running>()
                .Select(s => s.TotalQuantity)
                .SequenceEqual([10, 36.9, 37, 40])
        );
        var result = await process
            .State.OfType<BrewByWeightTracking.Finished>()
            .FirstOrDefaultAsync();
        Assert.NotNull(result);
        Assert.InRange(result.TotalQuantity, 39.9, 40.1);
    }

    [Theory]
    [InlineData(IBrewByWeightService.Spout.Double)]
    public async Task RunBrewByWeightDoubleSpoutSuccessTest(IBrewByWeightService.Spout spout)
    {
        var stateObserverMock = new Mock<IObserver<BrewByWeightState>>();
        var trackingObserverMock = new Mock<IObserver<BrewByWeightTracking>>();
        var paddleAccessMock = CreatePaddleAccessMock();
        var dbServiceMock = CreateBrewByWeightDbServiceMock();
        var scaleDataSubject = new Subject<ScaleDataPoint>();
        var scaleMock = CreateScaleConnectionMock(scaleDataSubject);
        var service = CreateService(
            3,
            paddleAccessMock.Object,
            scaleMock.Object,
            dbServiceMock.Object
        );
        service.State.Subscribe(stateObserverMock.Object);
        stateObserverMock.Verify(m => m.OnNext(It.IsAny<BrewByWeightState.Idle>()), Times.Once);
        var beanId = Guid.NewGuid();
        var process = service.RunBrewByWeight(beanId, 40, 1, 1, TimeSpan.Zero, spout);
        stateObserverMock.Verify(
            m => m.OnNext(new BrewByWeightState.Running(process.ProcessId)),
            Times.Once
        );
        process.State.Subscribe(trackingObserverMock.Object);
        trackingObserverMock.Verify(m => m.OnNext(new BrewByWeightTracking.Started()), Times.Once);
        scaleMock.Verify(m => m.TareAsync(It.IsAny<CancellationToken>()), Times.Once);
        paddleAccessMock.Verify(
            m => m.SetBrewPaddleOnAsync(true, It.IsAny<CancellationToken>()),
            Times.Never
        );
        scaleDataSubject.OnNext(new ScaleDataPoint(0, 0));
        paddleAccessMock.Verify(
            m => m.SetBrewPaddleOnAsync(true, It.IsAny<CancellationToken>()),
            Times.Once
        );
        paddleAccessMock.Verify(
            m => m.SetBrewPaddleOnAsync(false, It.IsAny<CancellationToken>()),
            Times.Never
        );
        scaleDataSubject.OnNext(new ScaleDataPoint(5, 0.5));
        paddleAccessMock.Verify(
            m => m.SetBrewPaddleOnAsync(false, It.IsAny<CancellationToken>()),
            Times.Never
        );
        scaleDataSubject.OnNext(new ScaleDataPoint(18.45, 0.5));
        paddleAccessMock.Verify(
            m => m.SetBrewPaddleOnAsync(false, It.IsAny<CancellationToken>()),
            Times.Never
        );
        scaleDataSubject.OnNext(new ScaleDataPoint(18.5, 0.5));
        paddleAccessMock.Verify(
            m => m.SetBrewPaddleOnAsync(false, It.IsAny<CancellationToken>()),
            Times.Once
        );
        scaleDataSubject.OnNext(new ScaleDataPoint(20, 0));
        await Task.Delay(TimeSpan.FromSeconds(3.1));
        trackingObserverMock.Verify(
            m => m.OnNext(It.IsAny<BrewByWeightTracking.Finished>()),
            Times.Once
        );
        trackingObserverMock.Verify(m => m.OnCompleted(), Times.Once);
        stateObserverMock.Verify(
            m => m.OnNext(It.IsAny<BrewByWeightState.Idle>()),
            Times.Exactly(2)
        );
        scaleMock.Verify(m => m.DisconnectAsync(It.IsAny<CancellationToken>()), Times.Once);
        dbServiceMock.Verify(m =>
            m.StoreProcessAsync(
                beanId,
                40,
                1,
                1,
                TimeSpan.Zero,
                spout,
                It.IsAny<IReadOnlyCollection<BrewByWeightTracking>>(),
                It.IsAny<CancellationToken>()
            )
        );
        Assert.True(
            await process
                .State.OfType<BrewByWeightTracking.Running>()
                .Select(s => s.Flow)
                .SequenceEqual([1, 1, 1, 0])
        );
        Assert.True(
            await process
                .State.OfType<BrewByWeightTracking.Running>()
                .Select(s => s.TotalQuantity)
                .SequenceEqual([10, 36.9, 37, 40])
        );
        var result = await process
            .State.OfType<BrewByWeightTracking.Finished>()
            .FirstOrDefaultAsync();
        Assert.NotNull(result);
        Assert.InRange(result.TotalQuantity, 39.9, 40.1);
    }

    [Fact]
    public async Task StopBrewByWeightTest()
    {
        var stateObserverMock = new Mock<IObserver<BrewByWeightState>>();
        var trackingObserverMock = new Mock<IObserver<BrewByWeightTracking>>();
        var paddleAccessMock = CreatePaddleAccessMock();
        var dbServiceMock = CreateBrewByWeightDbServiceMock();
        var scaleDataSubject = new Subject<ScaleDataPoint>();
        var scaleMock = CreateScaleConnectionMock(scaleDataSubject);
        var service = CreateService(
            3,
            paddleAccessMock.Object,
            scaleMock.Object,
            dbServiceMock.Object
        );
        service.State.Subscribe(stateObserverMock.Object);
        stateObserverMock.Verify(m => m.OnNext(It.IsAny<BrewByWeightState.Idle>()), Times.Once);
        var beanId = Guid.NewGuid();
        var process = service.RunBrewByWeight(
            beanId,
            40,
            1,
            1,
            TimeSpan.Zero,
            IBrewByWeightService.Spout.Single
        );
        stateObserverMock.Verify(
            m => m.OnNext(new BrewByWeightState.Running(process.ProcessId)),
            Times.Once
        );
        process.State.Subscribe(trackingObserverMock.Object);
        trackingObserverMock.Verify(m => m.OnNext(new BrewByWeightTracking.Started()), Times.Once);
        scaleMock.Verify(m => m.TareAsync(It.IsAny<CancellationToken>()), Times.Once);
        paddleAccessMock.Verify(
            m => m.SetBrewPaddleOnAsync(true, It.IsAny<CancellationToken>()),
            Times.Never
        );
        scaleDataSubject.OnNext(new ScaleDataPoint(0, 0));
        paddleAccessMock.Verify(
            m => m.SetBrewPaddleOnAsync(true, It.IsAny<CancellationToken>()),
            Times.Once
        );
        paddleAccessMock.Verify(
            m => m.SetBrewPaddleOnAsync(false, It.IsAny<CancellationToken>()),
            Times.Never
        );
        await service.StopBrewProcess(process.ProcessId);
        paddleAccessMock.Verify(
            m => m.SetBrewPaddleOnAsync(false, It.IsAny<CancellationToken>()),
            Times.Once
        );
        trackingObserverMock.Verify(
            m => m.OnNext(It.IsAny<BrewByWeightTracking.Cancelled>()),
            Times.Once
        );
        trackingObserverMock.Verify(m => m.OnCompleted(), Times.Once);
        stateObserverMock.Verify(
            m => m.OnNext(It.IsAny<BrewByWeightState.Idle>()),
            Times.Exactly(2)
        );
        scaleMock.Verify(m => m.DisconnectAsync(It.IsAny<CancellationToken>()), Times.Once);
        dbServiceMock.Verify(m =>
            m.StoreProcessAsync(
                beanId,
                40,
                1,
                1,
                TimeSpan.Zero,
                IBrewByWeightService.Spout.Single,
                It.IsAny<IReadOnlyCollection<BrewByWeightTracking>>(),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Fact]
    public void BrewByWeightScaleExceptionTest()
    {
        var stateObserverMock = new Mock<IObserver<BrewByWeightState>>();
        var trackingObserverMock = new Mock<IObserver<BrewByWeightTracking>>();
        var paddleAccessMock = CreatePaddleAccessMock();
        var dbServiceMock = CreateBrewByWeightDbServiceMock();
        var scaleDataSubject = new Subject<ScaleDataPoint>();
        var scaleMock = CreateScaleConnectionMock(scaleDataSubject);
        var service = CreateService(
            3,
            paddleAccessMock.Object,
            scaleMock.Object,
            dbServiceMock.Object,
            true
        );
        service.State.Subscribe(stateObserverMock.Object);
        stateObserverMock.Verify(m => m.OnNext(It.IsAny<BrewByWeightState.Idle>()), Times.Once);
        var beanId = Guid.NewGuid();
        var process = service.RunBrewByWeight(
            beanId,
            40,
            1,
            1,
            TimeSpan.Zero,
            IBrewByWeightService.Spout.Single
        );
        stateObserverMock.Verify(
            m => m.OnNext(new BrewByWeightState.Running(process.ProcessId)),
            Times.Once
        );
        process.State.Subscribe(trackingObserverMock.Object);
        trackingObserverMock.Verify(m => m.OnNext(new BrewByWeightTracking.Started()), Times.Once);
        paddleAccessMock.Verify(
            m => m.SetBrewPaddleOnAsync(true, It.IsAny<CancellationToken>()),
            Times.Never
        );
        trackingObserverMock.Verify(
            m => m.OnNext(It.IsAny<BrewByWeightTracking.Failed>()),
            Times.Once
        );
        trackingObserverMock.Verify(m => m.OnCompleted(), Times.Once);
        stateObserverMock.Verify(
            m => m.OnNext(It.IsAny<BrewByWeightState.Idle>()),
            Times.Exactly(2)
        );
        dbServiceMock.Verify(m =>
            m.StoreProcessAsync(
                beanId,
                40,
                1,
                1,
                TimeSpan.Zero,
                IBrewByWeightService.Spout.Single,
                It.IsAny<IReadOnlyCollection<BrewByWeightTracking>>(),
                It.IsAny<CancellationToken>()
            )
        );
    }

    [Fact]
    public async Task BrewByWeightMaximumDripTimeTest()
    {
        var stateObserverMock = new Mock<IObserver<BrewByWeightState>>();
        var trackingObserverMock = new Mock<IObserver<BrewByWeightTracking>>();
        var paddleAccessMock = CreatePaddleAccessMock();
        var dbServiceMock = CreateBrewByWeightDbServiceMock();
        var scaleDataSubject = new Subject<ScaleDataPoint>();
        var scaleMock = CreateScaleConnectionMock(scaleDataSubject);
        var service = CreateService(
            3,
            paddleAccessMock.Object,
            scaleMock.Object,
            dbServiceMock.Object
        );
        service.State.Subscribe(stateObserverMock.Object);
        stateObserverMock.Verify(m => m.OnNext(It.IsAny<BrewByWeightState.Idle>()), Times.Once);
        var process = service.RunBrewByWeight(
            Guid.NewGuid(),
            40,
            1,
            1,
            TimeSpan.Zero,
            IBrewByWeightService.Spout.Single
        );
        stateObserverMock.Verify(
            m => m.OnNext(new BrewByWeightState.Running(process.ProcessId)),
            Times.Once
        );
        process.State.Subscribe(trackingObserverMock.Object);
        trackingObserverMock.Verify(m => m.OnNext(new BrewByWeightTracking.Started()), Times.Once);
        scaleMock.Verify(m => m.TareAsync(It.IsAny<CancellationToken>()), Times.Once);
        paddleAccessMock.Verify(
            m => m.SetBrewPaddleOnAsync(true, It.IsAny<CancellationToken>()),
            Times.Never
        );
        scaleDataSubject.OnNext(new ScaleDataPoint(0, 0));
        paddleAccessMock.Verify(
            m => m.SetBrewPaddleOnAsync(true, It.IsAny<CancellationToken>()),
            Times.Once
        );
        paddleAccessMock.Verify(
            m => m.SetBrewPaddleOnAsync(false, It.IsAny<CancellationToken>()),
            Times.Never
        );
        scaleDataSubject.OnNext(new ScaleDataPoint(10, 1));
        paddleAccessMock.Verify(
            m => m.SetBrewPaddleOnAsync(false, It.IsAny<CancellationToken>()),
            Times.Never
        );
        scaleDataSubject.OnNext(new ScaleDataPoint(36.9, 1));
        paddleAccessMock.Verify(
            m => m.SetBrewPaddleOnAsync(false, It.IsAny<CancellationToken>()),
            Times.Never
        );
        scaleDataSubject.OnNext(new ScaleDataPoint(37, 1));
        paddleAccessMock.Verify(
            m => m.SetBrewPaddleOnAsync(false, It.IsAny<CancellationToken>()),
            Times.Once
        );
        await Task.Delay(TimeSpan.FromSeconds(10.1));
        scaleDataSubject.OnNext(new ScaleDataPoint(37, 1));
        await Task.Delay(TimeSpan.FromSeconds(3.1));
        trackingObserverMock.Verify(
            m => m.OnNext(It.IsAny<BrewByWeightTracking.Finished>()),
            Times.Once
        );
        trackingObserverMock.Verify(m => m.OnCompleted(), Times.Once);
        stateObserverMock.Verify(
            m => m.OnNext(It.IsAny<BrewByWeightState.Idle>()),
            Times.Exactly(2)
        );
        Assert.True(
            await process
                .State.OfType<BrewByWeightTracking.Running>()
                .Select(s => s.Flow)
                .SequenceEqual([1, 1, 1, 1])
        );
        Assert.True(
            await process
                .State.OfType<BrewByWeightTracking.Running>()
                .Select(s => s.TotalQuantity)
                .SequenceEqual([10, 36.9, 37, 37])
        );
        var result = await process
            .State.OfType<BrewByWeightTracking.Finished>()
            .FirstOrDefaultAsync();
        Assert.NotNull(result);
        Assert.InRange(result.TotalQuantity, 36.9, 37.1);
    }
}
