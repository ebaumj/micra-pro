namespace MicraPro.FlowProfiling.DataDefinition.ValueObjects;

public class FlowProfileException : Exception
{
    public class FlowRegulationNotAvailable : FlowProfileException;

    public class FlowProfilingServiceNotReady : FlowProfileException;

    public class UnknownFlowProfilingError : FlowProfileException;
}
