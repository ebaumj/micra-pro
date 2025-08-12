using MicraPro.FlowProfiling.Domain.HardwareAccess;
using MicraPro.FlowProfiling.Domain.Services;
using Moq;

namespace MicraPro.FlowProfiling.Domain.Test.Services;

public class FlowRampGeneratorServiceTest
{
    private Mock<IFlowRegulator> CreateFlowRegulatorMock()
    {
        double currentFlow = 0;
        var flowRegulatorMock = new Mock<IFlowRegulator>();
        flowRegulatorMock
            .Setup(m => m.SetFlow(It.IsAny<double>()))
            .Callback((double flow) => currentFlow = flow);
        flowRegulatorMock.Setup(m => m.CurrentFlow).Returns(() => currentFlow);
        return flowRegulatorMock;
    }

    [Fact]
    public async Task StartFlowRamp()
    {
        var regulator = CreateFlowRegulatorMock();
        var service = new FlowRampGeneratorService(regulator.Object);
        service.StartFlowRamp(1, TimeSpan.FromMilliseconds(200));
        await Task.Delay(TimeSpan.FromMilliseconds(110));
        regulator.Verify(m => m.SetFlow(0.5), Times.Once);
        await Task.Delay(TimeSpan.FromMilliseconds(100));
        regulator.Verify(m => m.SetFlow(1), Times.Once);
        await Task.Delay(TimeSpan.FromMilliseconds(100));
        Assert.InRange(regulator.Object.CurrentFlow, 0.99, 1.01);
        regulator.Verify(m => m.CurrentFlow);
        regulator.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task StopFlowRamp()
    {
        var regulator = CreateFlowRegulatorMock();
        var service = new FlowRampGeneratorService(regulator.Object);
        service.StartFlowRamp(1, TimeSpan.FromMilliseconds(200));
        await Task.Delay(TimeSpan.FromMilliseconds(110));
        regulator.Verify(m => m.SetFlow(0.5), Times.Once);
        service.StopFlowRamp();
        await Task.Delay(TimeSpan.FromMilliseconds(100));
        await Task.Delay(TimeSpan.FromMilliseconds(100));
        Assert.InRange(regulator.Object.CurrentFlow, 0.49, 0.51);
        regulator.Verify(m => m.CurrentFlow);
        regulator.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task StartSecondFlowRamp()
    {
        var regulator = CreateFlowRegulatorMock();
        var service = new FlowRampGeneratorService(regulator.Object);
        service.StartFlowRamp(1, TimeSpan.FromMilliseconds(200));
        await Task.Delay(TimeSpan.FromMilliseconds(110));
        regulator.Verify(m => m.SetFlow(0.5), Times.Once);
        await Task.Delay(TimeSpan.FromMilliseconds(100));
        regulator.Verify(m => m.SetFlow(1), Times.Once);
        await Task.Delay(TimeSpan.FromMilliseconds(100));
        Assert.InRange(regulator.Object.CurrentFlow, 0.99, 1.01);
        service.StartFlowRamp(1.5, TimeSpan.FromMilliseconds(200));
        await Task.Delay(TimeSpan.FromMilliseconds(110));
        regulator.Verify(m => m.SetFlow(1.25), Times.Once);
        await Task.Delay(TimeSpan.FromMilliseconds(100));
        regulator.Verify(m => m.SetFlow(1.5), Times.Once);
        await Task.Delay(TimeSpan.FromMilliseconds(100));
        Assert.InRange(regulator.Object.CurrentFlow, 1.49, 1.51);
        regulator.Verify(m => m.CurrentFlow);
        regulator.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task InterruptFlowRamp()
    {
        var regulator = CreateFlowRegulatorMock();
        var service = new FlowRampGeneratorService(regulator.Object);
        service.StartFlowRamp(1, TimeSpan.FromMilliseconds(200));
        await Task.Delay(TimeSpan.FromMilliseconds(110));
        regulator.Verify(m => m.SetFlow(0.5), Times.Once);
        service.StartFlowRamp(1.25, TimeSpan.FromMilliseconds(300));
        await Task.Delay(TimeSpan.FromMilliseconds(110));
        regulator.Verify(m => m.SetFlow(0.75), Times.Once);
        await Task.Delay(TimeSpan.FromMilliseconds(100));
        regulator.Verify(m => m.SetFlow(1), Times.Once);
        await Task.Delay(TimeSpan.FromMilliseconds(100));
        regulator.Verify(m => m.SetFlow(1.25), Times.Once);
        await Task.Delay(TimeSpan.FromMilliseconds(100));
        Assert.InRange(regulator.Object.CurrentFlow, 1.24, 1.26);
        regulator.Verify(m => m.CurrentFlow);
        regulator.VerifyNoOtherCalls();
    }
}
