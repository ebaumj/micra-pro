using System.Reactive.Linq;
using System.Reactive.Subjects;
using MicraPro.FlowProfiling.DataDefinition;
using MicraPro.FlowProfiling.DataDefinition.ValueObjects;
using MicraPro.FlowProfiling.Domain.HardwareAccess;
using MicraPro.FlowProfiling.Domain.Interfaces;
using MicraPro.FlowProfiling.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace MicraPro.FlowProfiling.Domain.Test.Services;

public class FlowProfilingServiceTest
{
    [Fact]
    public async Task FlatFlowProfilingTest()
    {
        var flowPublisherMock = new Mock<IFlowPublisher>();
        flowPublisherMock.Setup(m => m.Flow).Returns(Observable.Empty<double>());
        var stateObserverMock = new Mock<IObserver<FlowProfilingState>>();
        var trackingObserverMock = new Mock<IObserver<FlowProfileTracking>>();
        var flowRampGeneratorMock = new Mock<IFlowRampGeneratorService>();
        var service = new FlowProfilingService(
            flowPublisherMock.Object,
            flowRampGeneratorMock.Object,
            Mock.Of<ILogger<FlowProfilingService>>()
        );
        service.State.Subscribe(stateObserverMock.Object);
        stateObserverMock.Verify(m => m.OnNext(It.IsAny<FlowProfilingState.Idle>()), Times.Once);
        var process = service.RunFlowProfiling(1, []);
        process.State.Subscribe(trackingObserverMock.Object);
        stateObserverMock.Verify(
            m => m.OnNext(new FlowProfilingState.Running(process.ProcessId)),
            Times.Once
        );
        trackingObserverMock.Verify(m => m.OnNext(It.IsAny<FlowProfileTracking.Started>()));
        flowRampGeneratorMock.Verify(m => m.StartFlowRamp(1, TimeSpan.Zero));
        await service.StopFlowProfilingProcess(process.ProcessId);
        await Task.Delay(TimeSpan.FromMilliseconds(10));
        flowRampGeneratorMock.Verify(m => m.StopFlowRamp());
        trackingObserverMock.Verify(
            m => m.OnNext(It.IsAny<FlowProfileTracking.ProfileDone>()),
            Times.Once
        );
        stateObserverMock.Verify(
            m => m.OnNext(It.IsAny<FlowProfilingState.Idle>()),
            Times.Exactly(2)
        );
        trackingObserverMock.Verify(m => m.OnNext(It.IsAny<FlowProfileTracking.Finished>()));
        trackingObserverMock.Verify(m => m.OnCompleted());
        trackingObserverMock.VerifyNoOtherCalls();
        stateObserverMock.VerifyNoOtherCalls();
        flowRampGeneratorMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RunFlowProfilingTest()
    {
        var flowPublisherMock = new Mock<IFlowPublisher>();
        flowPublisherMock.Setup(m => m.Flow).Returns(Observable.Empty<double>());
        var stateObserverMock = new Mock<IObserver<FlowProfilingState>>();
        var trackingObserverMock = new Mock<IObserver<FlowProfileTracking>>();
        var flowRampGeneratorMock = new Mock<IFlowRampGeneratorService>();
        var service = new FlowProfilingService(
            flowPublisherMock.Object,
            flowRampGeneratorMock.Object,
            Mock.Of<ILogger<FlowProfilingService>>()
        );
        service.State.Subscribe(stateObserverMock.Object);
        stateObserverMock.Verify(m => m.OnNext(It.IsAny<FlowProfilingState.Idle>()), Times.Once);
        var process = service.RunFlowProfiling(
            0.5,
            [
                new IFlowProfilingService.FlowDataPoint(1, TimeSpan.FromMilliseconds(50)),
                new IFlowProfilingService.FlowDataPoint(2, TimeSpan.FromMilliseconds(100)),
            ]
        );
        process.State.Subscribe(trackingObserverMock.Object);
        stateObserverMock.Verify(
            m => m.OnNext(new FlowProfilingState.Running(process.ProcessId)),
            Times.Once
        );
        trackingObserverMock.Verify(m => m.OnNext(It.IsAny<FlowProfileTracking.Started>()));
        flowRampGeneratorMock.Verify(m => m.StartFlowRamp(0.5, TimeSpan.Zero));
        flowRampGeneratorMock.Verify(m => m.StartFlowRamp(1, TimeSpan.FromMilliseconds(50)));
        await Task.Delay(TimeSpan.FromMilliseconds(60));
        flowRampGeneratorMock.Verify(m => m.StartFlowRamp(2, TimeSpan.FromMilliseconds(50)));
        await Task.Delay(TimeSpan.FromMilliseconds(50));
        await service.StopFlowProfilingProcess(process.ProcessId);
        await Task.Delay(TimeSpan.FromMilliseconds(10));
        flowRampGeneratorMock.Verify(m => m.StopFlowRamp());
        trackingObserverMock.Verify(
            m => m.OnNext(It.IsAny<FlowProfileTracking.ProfileDone>()),
            Times.Once
        );
        stateObserverMock.Verify(
            m => m.OnNext(It.IsAny<FlowProfilingState.Idle>()),
            Times.Exactly(2)
        );
        trackingObserverMock.Verify(m => m.OnNext(It.IsAny<FlowProfileTracking.Finished>()));
        trackingObserverMock.Verify(m => m.OnCompleted());
        trackingObserverMock.VerifyNoOtherCalls();
        stateObserverMock.VerifyNoOtherCalls();
        flowRampGeneratorMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task StopFlowProfilingTest()
    {
        var flowPublisherMock = new Mock<IFlowPublisher>();
        flowPublisherMock.Setup(m => m.Flow).Returns(Observable.Empty<double>());
        var stateObserverMock = new Mock<IObserver<FlowProfilingState>>();
        var trackingObserverMock = new Mock<IObserver<FlowProfileTracking>>();
        var flowRampGeneratorMock = new Mock<IFlowRampGeneratorService>();
        var service = new FlowProfilingService(
            flowPublisherMock.Object,
            flowRampGeneratorMock.Object,
            Mock.Of<ILogger<FlowProfilingService>>()
        );
        service.State.Subscribe(stateObserverMock.Object);
        stateObserverMock.Verify(m => m.OnNext(It.IsAny<FlowProfilingState.Idle>()), Times.Once);
        var process = service.RunFlowProfiling(
            0.5,
            [
                new IFlowProfilingService.FlowDataPoint(1, TimeSpan.FromMilliseconds(50)),
                new IFlowProfilingService.FlowDataPoint(2, TimeSpan.FromMilliseconds(100)),
            ]
        );
        process.State.Subscribe(trackingObserverMock.Object);
        stateObserverMock.Verify(
            m => m.OnNext(new FlowProfilingState.Running(process.ProcessId)),
            Times.Once
        );
        trackingObserverMock.Verify(m => m.OnNext(It.IsAny<FlowProfileTracking.Started>()));
        flowRampGeneratorMock.Verify(m => m.StartFlowRamp(0.5, TimeSpan.Zero));
        flowRampGeneratorMock.Verify(m => m.StartFlowRamp(1, TimeSpan.FromMilliseconds(50)));
        await service.StopFlowProfilingProcess(process.ProcessId);
        await Task.Delay(TimeSpan.FromMilliseconds(10));
        flowRampGeneratorMock.Verify(m => m.StopFlowRamp());
        trackingObserverMock.Verify(
            m => m.OnNext(It.IsAny<FlowProfileTracking.ProfileDone>()),
            Times.Never
        );
        stateObserverMock.Verify(
            m => m.OnNext(It.IsAny<FlowProfilingState.Idle>()),
            Times.Exactly(2)
        );
        trackingObserverMock.Verify(m => m.OnNext(It.IsAny<FlowProfileTracking.Cancelled>()));
        trackingObserverMock.Verify(m => m.OnCompleted());
        trackingObserverMock.VerifyNoOtherCalls();
        stateObserverMock.VerifyNoOtherCalls();
        flowRampGeneratorMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task TrackFlowProfilingTest()
    {
        var flowPublisherMock = new Mock<IFlowPublisher>();
        var flowSubject = new Subject<double>();
        flowPublisherMock.Setup(m => m.Flow).Returns(flowSubject);
        var trackingObserverMock = new Mock<IObserver<FlowProfileTracking>>();
        var flowRampGeneratorMock = new Mock<IFlowRampGeneratorService>();
        var service = new FlowProfilingService(
            flowPublisherMock.Object,
            flowRampGeneratorMock.Object,
            Mock.Of<ILogger<FlowProfilingService>>()
        );
        var process = service.RunFlowProfiling(0.5, []);
        process.State.Subscribe(trackingObserverMock.Object);
        trackingObserverMock.Verify(m => m.OnNext(It.IsAny<FlowProfileTracking.Started>()));
        trackingObserverMock.Verify(m => m.OnNext(It.IsAny<FlowProfileTracking.ProfileDone>()));
        flowSubject.OnNext(0.125);
        flowSubject.OnNext(0.25);
        flowSubject.OnNext(0.5);
        flowSubject.OnNext(1);
        flowSubject.OnNext(3);
        await service.StopFlowProfilingProcess(process.ProcessId);
        await Task.Delay(TimeSpan.FromMilliseconds(10));
        trackingObserverMock.Verify(m => m.OnNext(It.IsAny<FlowProfileTracking.Running>()));
        trackingObserverMock.Verify(m => m.OnNext(It.IsAny<FlowProfileTracking.Finished>()));
        trackingObserverMock.Verify(m => m.OnCompleted());
        Assert.True(
            await process
                .State.OfType<FlowProfileTracking.Running>()
                .Select(s => s.Flow)
                .SequenceEqual([0.125, 0.25, 0.5, 1, 3])
                .FirstAsync()
        );
        trackingObserverMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task FlowProfilingResultTest()
    {
        var flowPublisherMock = new Mock<IFlowPublisher>();
        var flowSubject = new Subject<double>();
        flowPublisherMock.Setup(m => m.Flow).Returns(flowSubject);
        var flowRampGeneratorMock = new Mock<IFlowRampGeneratorService>();
        var trackingObserverMock = new Mock<IObserver<FlowProfileTracking>>();
        var service = new FlowProfilingService(
            flowPublisherMock.Object,
            flowRampGeneratorMock.Object,
            Mock.Of<ILogger<FlowProfilingService>>()
        );
        var process = service.RunFlowProfiling(0.5, []);
        process.State.Subscribe(trackingObserverMock.Object);
        await Task.Delay(TimeSpan.FromMilliseconds(100));
        flowSubject.OnNext(2.5);
        await Task.Delay(TimeSpan.FromMilliseconds(100));
        flowSubject.OnNext(1);
        await Task.Delay(TimeSpan.FromMilliseconds(100));
        flowSubject.OnNext(1);
        await service.StopFlowProfilingProcess(process.ProcessId);
        await Task.Delay(TimeSpan.FromMilliseconds(10));
        trackingObserverMock.Verify(m => m.OnNext(It.IsAny<FlowProfileTracking.Finished>()));
        var result = await process.State.OfType<FlowProfileTracking.Finished>().FirstAsync();
        Assert.InRange(
            result.TotalTime,
            TimeSpan.FromMilliseconds(250),
            TimeSpan.FromMilliseconds(350)
        );
        Assert.InRange(result.AverageFlow, 1.45, 1.55);
    }
}
