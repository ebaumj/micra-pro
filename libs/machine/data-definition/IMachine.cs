using MicraPro.Machine.DataDefinition.ValueObjects;

namespace MicraPro.Machine.DataDefinition;

public interface IMachine
{
    IObservable<bool> IsStandby { get; }
    Task SetStandbyAsync(bool standby, CancellationToken ct);

    Task SetSmartStandbyAsync(SmartStandby? standby, CancellationToken ct);
    Task<SmartStandby?> GetSmartStandbyAsync(CancellationToken ct);

    Task<Boilers> GetBoilersAsync(CancellationToken ct);
    Task SetBoilerTargetTemperatureAsync(int temperature, CancellationToken ct);
    Task SetSteamLevelAsync(int level, CancellationToken ct);
    Task SetSteamBoilerEnabledAsync(bool enabled, CancellationToken ct);
}
