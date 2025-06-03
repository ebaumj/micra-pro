using Linux.Bluetooth;
using Linux.Bluetooth.Extensions;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.ScaleManagement.Infrastructure.BluetoothAccess.LinuxBluetooth;

public class BleDeviceConnection(IDevice1 device) : IBleDeviceConnection
{
    public async Task<IBleService> GetServiceAsync(Guid serviceId, CancellationToken ct) =>
        await CreateServiceAsync(await device.GetServiceAsync(serviceId.ToString()));

    public async Task<IBleService[]> GetServicesAsync(CancellationToken ct) =>
        (
            await (await device.GetServicesAsync().WaitAsync(ct)).SelectAsync(CreateServiceAsync)
        ).ToArray();

    public Task Disconnect(CancellationToken ct) => device.DisconnectAsync().WaitAsync(ct);

    private static async Task<IBleService> CreateServiceAsync(IGattService1 service) =>
        new BleService(service, Guid.Parse(await service.GetUUIDAsync()));

    public void Dispose()
    {
        device.DisconnectAsync();
    }
}
