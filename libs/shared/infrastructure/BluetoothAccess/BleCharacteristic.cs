using System.Reactive.Linq;
using Linux.Bluetooth;
using MicraPro.Shared.Domain.BluetoothAccess;
using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.Shared.Infrastructure.BluetoothAccess;

public class BleCharacteristic(GattCharacteristic characteristic) : IBleCharacteristic
{
    public Task WriteValueAsync(byte[] data, CancellationToken ct) =>
        characteristic.WriteValueAsync(data, new Dictionary<string, object>()).WaitAsync(ct);

    public Task<byte[]> ReadValueAsync(CancellationToken ct) =>
        characteristic.ReadValueAsync(new Dictionary<string, object>()).WaitAsync(ct);

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
