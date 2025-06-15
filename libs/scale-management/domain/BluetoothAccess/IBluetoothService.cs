namespace MicraPro.ScaleManagement.Domain.BluetoothAccess;

public interface IBluetoothService
{
    IObservable<BluetoothScanResult> DetectedDevices { get; }
    IObservable<bool> IsScanning { get; }
    Task DiscoverAsync(TimeSpan discoveryTime, CancellationToken ct);
    Task<IBleDeviceConnection> ConnectDeviceAsync(string deviceId, CancellationToken ct);
}
