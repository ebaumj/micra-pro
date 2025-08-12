using System.Reactive.Disposables;
using System.Reactive.Linq;
using MicraPro.FlowProfiling.Domain.HardwareAccess;
using MicraPro.FlowProfiling.Domain.Interfaces;

namespace MicraPro.FlowProfiling.Domain.Services;

public class FlowRampGeneratorService(IFlowRegulator flowRegulator) : IFlowRampGeneratorService
{
    private static readonly TimeSpan RampUpdateInterval = TimeSpan.FromMilliseconds(100);

    private IDisposable _currentRamp = Disposable.Empty;

    public void StartFlowRamp(double targetFlow, TimeSpan duration)
    {
        _currentRamp.Dispose();
        var currentFlow = flowRegulator.CurrentFlow;
        var flowDifference = targetFlow - currentFlow;
        if (duration < RampUpdateInterval)
            flowRegulator.SetFlow(targetFlow);
        else
            _currentRamp = Observable
                .Interval(RampUpdateInterval)
                .Take((int)(duration / RampUpdateInterval))
                .Subscribe(i =>
                {
                    flowRegulator.SetFlow(
                        currentFlow
                            + flowDifference * (i + 1) / (int)(duration / RampUpdateInterval)
                    );
                });
    }

    public void StopFlowRamp()
    {
        _currentRamp.Dispose();
    }
}
