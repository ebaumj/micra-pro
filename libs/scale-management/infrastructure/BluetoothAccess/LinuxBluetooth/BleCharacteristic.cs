using System.Reactive.Linq;
using Linux.Bluetooth;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.ScaleManagement.Infrastructure.BluetoothAccess.LinuxBluetooth;

public class BleCharacteristic(GattCharacteristic characteristic) : IBleCharacteristic
{
    public Task SendCommandAsync(byte[] data, CancellationToken ct) =>
        characteristic.WriteValueAsync(data, new Dictionary<string, object>());

    public Task<IObservable<byte[]>> GetValueObservableAsync(CancellationToken ct) =>
        Task.FromResult<IObservable<byte[]>>(
            new LifecycleAwareObservable<byte[]>(
                Observable.FromEvent<GattCharacteristicEventHandlerAsync, byte[]>(
                    handler =>
                        (_, e) =>
                        {
                            handler(e.Value);
                            return Task.CompletedTask;
                        },
                    evHandler => characteristic.Value += evHandler,
                    evHandler => characteristic.Value -= evHandler
                ),
                () =>
                {
                    characteristic.StartNotifyAsync();
                },
                () =>
                {
                    characteristic.StopNotifyAsync();
                }
            )
        );
}
