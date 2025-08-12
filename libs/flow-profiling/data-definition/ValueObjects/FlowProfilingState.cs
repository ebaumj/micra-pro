namespace MicraPro.FlowProfiling.DataDefinition.ValueObjects;

public abstract record FlowProfilingState
{
    public record Idle : FlowProfilingState;

    public record Running(Guid ProcessId) : FlowProfilingState;
}
