using System.Reactive.Linq;
using MicraPro.Machine.Domain.BluetoothAccess;

namespace MicraPro.Machine.Infrastructure.BluetoothAccess;

public class BluetoothService(Shared.Domain.BluetoothAccess.IBluetoothService service)
    : IBluetoothService
{
    private const string NamePrefix = "MICRA";

    public IObservable<IBluetoothService.ScanResult> DetectedDevices =>
        service
            .DetectedDevices.Where(d => d.Name.StartsWith(NamePrefix))
            .Select(d => new IBluetoothService.ScanResult(d.Name, d.DeviceId));
    public IObservable<bool> IsScanning => service.IsScanning;

    public Task DiscoverAsync(TimeSpan discoveryTime, CancellationToken ct) =>
        service.DiscoverAsync(discoveryTime, ct);

    public async Task<IBluetoothConnection> ConnectAsync(string bluetoothId, CancellationToken ct)
    {
        var connection = new BluetoothConnection(await service.ConnectDeviceAsync(bluetoothId, ct));
        await connection.SetupAsync(ct);
        return connection;
    }
}
