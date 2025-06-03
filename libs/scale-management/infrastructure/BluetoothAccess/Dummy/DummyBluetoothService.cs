using System.Reactive.Subjects;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using MicraPro.ScaleManagement.Domain.ScaleImplementations.BookooThemisMini;

namespace MicraPro.ScaleManagement.Infrastructure.BluetoothAccess.Dummy;

public class DummyBluetoothService : IBluetoothService
{
    public IObservable<BluetoothScanResult> DetectedDevices =>
        new BehaviorSubject<BluetoothScanResult>(
            new BluetoothScanResult("Dummy Bookoo Scale", "Dummy ID", Scale.RequiredServiceIds)
        );
    public IObservable<bool> IsScanning => new BehaviorSubject<bool>(false);

    public Task DiscoverAsync(TimeSpan discoveryTime, CancellationToken ct) => Task.CompletedTask;

    public Task<IBleDeviceConnection> ConnectDeviceAsync(string deviceId, CancellationToken ct) =>
        Task.FromResult<IBleDeviceConnection>(new DummyConnection());
}
