using System.Reactive.Linq;
using Linux.Bluetooth;
using Linux.Bluetooth.Extensions;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;

namespace MicraPro.ScaleManagement.Infrastructure.BluetoothAccess.LinuxBluetooth;

public class BleDeviceConnection(IDevice1 device) : IBleDeviceConnection
{
    private static readonly TimeSpan ServiceSearchTimeout = TimeSpan.FromSeconds(1);

    public async Task<IBleService> GetServiceAsync(string serviceId, CancellationToken ct)
    {
        bool resolved;
        do
        {
            resolved = await device.GetServicesResolvedAsync().WaitAsync(ServiceSearchTimeout, ct);
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

    public void Dispose()
    {
        Observable.FromAsync(async ct => await Disconnect(ct)).Subscribe(_ => { });
    }
}
