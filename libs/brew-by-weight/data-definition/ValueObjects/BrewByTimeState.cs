namespace MicraPro.BrewByWeight.DataDefinition.ValueObjects;

public record BrewByTimeState
{
    public record Idle : BrewByTimeState;

    public record Running(Guid ProcessId) : BrewByTimeState;
}
