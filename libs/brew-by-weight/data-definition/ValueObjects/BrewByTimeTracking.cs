namespace MicraPro.BrewByWeight.DataDefinition.ValueObjects;

public abstract record BrewByTimeTracking
{
    public record Started : BrewByTimeTracking;

    public record Running(TimeSpan TargetTime, TimeSpan TotalTime) : BrewByTimeTracking();

    public record Finished(TimeSpan TargetTime, TimeSpan ExtractionTime) : BrewByTimeTracking;

    public record Cancelled(TimeSpan TargetTime, TimeSpan TotalTime) : BrewByTimeTracking;

    public record Failed(BrewByWeightException Exception, TimeSpan TargetTime, TimeSpan TotalTime)
        : BrewByTimeTracking;
}
