using InTheHand.Bluetooth;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;

namespace MicraPro.ScaleManagement.Infrastructure.BluetoothAccess.InTheHand;

public class BleDeviceConnection(BluetoothDevice device) : IBleDeviceConnection
{
    public async Task<IBleService> GetServiceAsync(Guid serviceId, CancellationToken ct) =>
        new BleService(await device.Gatt.GetPrimaryServiceAsync(serviceId).WaitAsync(ct));

    public async Task<IBleService[]> GetServicesAsync(CancellationToken ct) =>
        (await device.Gatt.GetPrimaryServicesAsync().WaitAsync(ct))
            .Select(s => new BleService(s))
            .Cast<IBleService>()
            .ToArray();

    public Task Disconnect(CancellationToken ct)
    {
        device.Gatt.Disconnect();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        device.Gatt.Disconnect();
    }
}
