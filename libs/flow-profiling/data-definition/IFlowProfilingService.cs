using MicraPro.FlowProfiling.DataDefinition.ValueObjects;

namespace MicraPro.FlowProfiling.DataDefinition;

public interface IFlowProfilingService
{
    public record FlowDataPoint(double Flow, TimeSpan Time);

    bool IsAvailable { get; }

    IObservable<FlowProfilingState> State { get; }

    IFlowProfilingProcess RunFlowProfiling(double startFlow, FlowDataPoint[] dataPoints);

    Task StopFlowProfilingProcess(Guid processId);
}
