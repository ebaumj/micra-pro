using System.Reactive.Linq;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;

namespace MicraPro.ScaleManagement.Infrastructure.BluetoothAccess.Dummy;

public class DummyBluetoothService : IBluetoothService
{
    public IObservable<BluetoothScanResult> DetectedDevices =>
        Observable.Empty<BluetoothScanResult>();
    public IObservable<bool> IsScanning => Observable.Return(false);

    public Task DiscoverAsync(TimeSpan discoveryTime, CancellationToken ct) => Task.CompletedTask;

    public Task<BluetoothScanResult[]> ScanDevicesAsync(
        TimeSpan scanTimeout,
        CancellationToken ct
    ) => Task.FromResult(Array.Empty<BluetoothScanResult>());

    public Task<IBleDeviceConnection> ConnectDeviceAsync(string deviceId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}
