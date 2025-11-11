using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using SharedBluetooth = MicraPro.Shared.Domain.BluetoothAccess;

namespace MicraPro.ScaleManagement.Infrastructure.BluetoothAccess.LinuxBluetooth;

public class BleDeviceConnection(SharedBluetooth.IBleDeviceConnection connection)
    : IBleDeviceConnection
{
    public async Task<IBleService> GetServiceAsync(string serviceId, CancellationToken ct) =>
        new BleService(await connection.GetServiceAsync(serviceId, ct));

    public async Task Disconnect(CancellationToken ct) => await connection.Disconnect(ct);
}
