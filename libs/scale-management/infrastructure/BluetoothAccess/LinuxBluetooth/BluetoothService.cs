using System.Reactive.Linq;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using SharedBluetooth = MicraPro.Shared.Domain.BluetoothAccess;

namespace MicraPro.ScaleManagement.Infrastructure.BluetoothAccess.LinuxBluetooth;

public class BluetoothService(SharedBluetooth.IBluetoothService bluetoothService)
    : IBluetoothService
{
    public IObservable<BluetoothScanResult> DetectedDevices =>
        bluetoothService.DetectedDevices.Select(d => new BluetoothScanResult(
            d.Name,
            d.DeviceId,
            d.ServiceIds
        ));
    public IObservable<bool> IsScanning => bluetoothService.IsScanning;

    public Task DiscoverAsync(TimeSpan discoveryTime, CancellationToken ct) =>
        bluetoothService.DiscoverAsync(discoveryTime, ct);

    public async Task<IBleDeviceConnection> ConnectDeviceAsync(
        string deviceId,
        CancellationToken ct
    ) => new BleDeviceConnection(await bluetoothService.ConnectDeviceAsync(deviceId, ct));
}
