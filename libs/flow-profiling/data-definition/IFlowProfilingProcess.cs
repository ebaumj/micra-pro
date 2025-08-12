using MicraPro.FlowProfiling.DataDefinition.ValueObjects;

namespace MicraPro.FlowProfiling.DataDefinition;

public interface IFlowProfilingProcess
{
    Guid ProcessId { get; }
    IObservable<FlowProfileTracking> State { get; }
}
