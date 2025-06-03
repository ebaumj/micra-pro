using System.Reactive.Linq;
using InTheHand.Bluetooth;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.ScaleManagement.Infrastructure.BluetoothAccess.InTheHand;

public class BleCharacteristic(GattCharacteristic characteristic) : IBleCharacteristic
{
    public Guid CharacteristicId => characteristic.Uuid;

    public Task SendCommandAsync(byte[] data, CancellationToken ct) =>
        characteristic.WriteValueWithoutResponseAsync(data).WaitAsync(ct);

    public Task<IObservable<byte[]>> GetValueObservableAsync(CancellationToken ct) =>
        Task.FromResult<IObservable<byte[]>>(
            new LifecycleAwareObservable<byte[]>(
                Observable.FromEvent<EventHandler<GattCharacteristicValueChangedEventArgs>, byte[]>(
                    handler =>
                        (_, e) =>
                        {
                            if (e.Error != null)
                                throw e.Error;
                            if (e.Value != null)
                                handler(e.Value);
                        },
                    evHandler => characteristic.CharacteristicValueChanged += evHandler,
                    evHandler => characteristic.CharacteristicValueChanged -= evHandler
                ),
                () =>
                {
                    characteristic.StartNotificationsAsync();
                },
                () =>
                {
                    characteristic.StopNotificationsAsync();
                }
            )
        );
}
