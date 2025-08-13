namespace MicraPro.BrewByWeight.DataDefinition.ValueObjects;

public abstract record BrewByWeightState
{
    public record Idle : BrewByWeightState;

    public record Running(Guid ProcessId) : BrewByWeightState;
}
