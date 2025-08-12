namespace MicraPro.FlowProfiling.DataDefinition.ValueObjects;

public record FlowProfileTracking
{
    public record Started : FlowProfileTracking;

    public record ProfileDone : FlowProfileTracking;

    public record Running(double Flow, TimeSpan TotalTime) : FlowProfileTracking;

    public record Finished(double AverageFlow, TimeSpan TotalTime) : FlowProfileTracking;

    public record Cancelled(double AverageFlow, TimeSpan TotalTime) : FlowProfileTracking;

    public record Failed(FlowProfileException Exception, double AverageFlow, TimeSpan TotalTime)
        : FlowProfileTracking;
}
