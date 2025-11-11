using Linux.Bluetooth;
using Linux.Bluetooth.Extensions;
using MicraPro.Shared.Domain.BluetoothAccess;

namespace MicraPro.Shared.Infrastructure.BluetoothAccess;

public class BleDeviceConnection(IDevice1 device) : IBleDeviceConnection
{
    private static readonly TimeSpan ServiceSearchTimeout = TimeSpan.FromSeconds(1);

    public async Task<IBleCharacteristic> GetCharacteristicAsync(
        string characteristicId,
        CancellationToken ct
    )
    {
        bool resolved;
        do
        {
            resolved = await device.GetServicesResolvedAsync().WaitAsync(ServiceSearchTimeout, ct);
            if (!await device.GetConnectedAsync().WaitAsync(ServiceSearchTimeout, ct))
                throw new InvalidOperationException("Device Disconnected!");
        } while (!resolved);
        foreach (var service in await device.GetServicesAsync().WaitAsync(ct))
        {
            try
            {
                return await new BleService(service).GetCharacteristicAsync(characteristicId, ct);
            }
            catch
            {
                // Characteristic not found, continue with next service
            }
        }
        throw new InvalidOperationException("Characteristic not found!");
    }

    public async Task<IBleService> GetServiceAsync(string serviceId, CancellationToken ct)
    {
        bool resolved;
        do
        {
            resolved = await device.GetServicesResolvedAsync().WaitAsync(ServiceSearchTimeout, ct);
            if (!await device.GetConnectedAsync().WaitAsync(ServiceSearchTimeout, ct))
                throw new InvalidOperationException("Device Disconnected!");
        } while (!resolved);
        return new BleService(await device.GetServiceAsync(serviceId));
    }

    public async Task Disconnect(CancellationToken ct)
    {
        try
        {
            await device.DisconnectAsync();
        }
        catch
        {
            // device is disconnected
        }
    }
}
