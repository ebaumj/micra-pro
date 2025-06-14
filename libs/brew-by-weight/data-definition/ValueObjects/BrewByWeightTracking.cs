namespace MicraPro.BrewByWeight.DataDefinition.ValueObjects;

public abstract record BrewByWeightTracking
{
    public record Started : BrewByWeightTracking;

    public record Running(double Flow, double TotalQuantity, TimeSpan TotalTime)
        : BrewByWeightTracking();

    public record Finished(double AverageFlow, double TotalQuantity, TimeSpan TotalTime)
        : BrewByWeightTracking;

    public record Cancelled(double AverageFlow, double TotalQuantity, TimeSpan TotalTime)
        : BrewByWeightTracking;

    public record Failed(
        BrewByWeightException Exception,
        double AverageFlow,
        double TotalQuantity,
        TimeSpan TotalTime
    ) : BrewByWeightTracking;
}
