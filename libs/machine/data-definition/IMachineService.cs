namespace MicraPro.Machine.DataDefinition;

public interface IMachineService
{
    public record MachineScanResult(string Name, string Id);

    IObservable<IMachine?> MachineObservable { get; }
    IMachine? Machine { get; }
    Task ConnectAsync(string id, CancellationToken ct);
    Task DisconnectAsync(CancellationToken ct);
    IObservable<MachineScanResult> Scan(CancellationToken ct);
}
