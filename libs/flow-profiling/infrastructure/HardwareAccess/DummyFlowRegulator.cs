using System.Reactive.Linq;
using MicraPro.FlowProfiling.Domain.HardwareAccess;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MicraPro.FlowProfiling.Infrastructure.HardwareAccess;

internal class DummyFlowRegulator(
    ILogger<DummyFlowRegulator> logger,
    IOptions<FlowProfilingInfrastructureOptions> options
) : IFlowPublisher, IFlowRegulator
{
    public IObservable<double> Flow =>
        Observable.Interval(TimeSpan.FromMilliseconds(50)).Select(_ => CurrentFlow);
    public double CurrentFlow { get; private set; }

    public void SetFlow(double flow)
    {
        logger.LogInformation("Set Flow: {f}", flow);
        CurrentFlow = flow;
    }

    public void StopRegulation()
    {
        logger.LogInformation("Stop Flow Regulator");
    }

    public bool IsAvailable => options.Value.IsAvailable;
}
