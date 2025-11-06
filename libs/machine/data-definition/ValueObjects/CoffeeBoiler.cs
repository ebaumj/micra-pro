namespace MicraPro.Machine.DataDefinition.ValueObjects;

public record CoffeeBoiler(bool IsEnabled, int TargetTemperature, int Temperature);
