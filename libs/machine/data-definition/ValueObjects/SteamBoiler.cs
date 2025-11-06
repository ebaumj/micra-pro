namespace MicraPro.Machine.DataDefinition.ValueObjects;

public record SteamBoiler(bool IsEnabled, int TargetLevel, int Temperature);
