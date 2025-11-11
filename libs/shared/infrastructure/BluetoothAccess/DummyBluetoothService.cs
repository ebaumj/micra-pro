using System.Reactive.Linq;
using MicraPro.Shared.Domain.BluetoothAccess;

namespace MicraPro.Shared.Infrastructure.BluetoothAccess;

public class DummyBluetoothService : IBluetoothService
{
    public IObservable<IBluetoothService.BluetoothScanResult> DetectedDevices =>
        Observable.Empty<IBluetoothService.BluetoothScanResult>();
    public IObservable<bool> IsScanning => Observable.Return(false);

    public Task DiscoverAsync(TimeSpan discoveryTime, CancellationToken ct) => Task.CompletedTask;

    public Task<IBleDeviceConnection> ConnectDeviceAsync(string deviceId, CancellationToken ct) =>
        throw new NotImplementedException();
}
