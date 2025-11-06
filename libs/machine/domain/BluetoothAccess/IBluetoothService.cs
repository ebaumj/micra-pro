namespace MicraPro.Machine.Domain.BluetoothAccess;

public interface IBluetoothService
{
    public record ScanResult(string Name, string BluetoothId);

    IObservable<ScanResult> DetectedDevices { get; }
    IObservable<bool> IsScanning { get; }
    Task DiscoverAsync(TimeSpan discoveryTime, CancellationToken ct);
    Task<IBluetoothConnection> ConnectAsync(string bluetoothId, CancellationToken ct);
}
