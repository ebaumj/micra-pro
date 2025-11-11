namespace MicraPro.Shared.Domain.BluetoothAccess;

public interface IBluetoothService
{
    public record BluetoothScanResult(string Name, string DeviceId, string[] ServiceIds);

    IObservable<BluetoothScanResult> DetectedDevices { get; }
    IObservable<bool> IsScanning { get; }
    Task DiscoverAsync(TimeSpan discoveryTime, CancellationToken ct);
    Task<IBleDeviceConnection> ConnectDeviceAsync(string deviceId, CancellationToken ct);
}
