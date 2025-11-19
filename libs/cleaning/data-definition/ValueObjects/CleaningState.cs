namespace MicraPro.Cleaning.DataDefinition.ValueObjects;

public abstract record CleaningState
{
    public record Started : CleaningState;

    public record Finished(TimeSpan TotalTime, int TotalCycles) : CleaningState;

    public record Cancelled(TimeSpan TotalTime, int TotalCycles) : CleaningState;

    public record Failed(TimeSpan TotalTime, int TotalCycles) : CleaningState;

    public record Running(TimeSpan TotalTime, int CycleNumber, TimeSpan CycleTime) : CleaningState;
}
