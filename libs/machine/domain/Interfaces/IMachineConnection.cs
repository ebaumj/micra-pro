namespace MicraPro.Machine.Domain.Interfaces;

public interface IMachineConnection
{
    public enum ReadSetting
    {
        MachineCapabilities,
        MachineMode,
        TankStatus,
        Boilers,
        SmartStandBy,
    }

    Task<string> ReadValueAsync(ReadSetting readSetting, CancellationToken ct);
    Task WriteValueAsync(string name, object data, CancellationToken ct);
    Task DisconnectAsync(CancellationToken ct);
    IObservable<bool> IsStandby { get; }
}
