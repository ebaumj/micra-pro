using Linux.Bluetooth;
using Linux.Bluetooth.Extensions;
using MicraPro.Shared.Domain.BluetoothAccess;

namespace MicraPro.Shared.Infrastructure.BluetoothAccess;

public class BleService(IGattService1 service) : IBleService
{
    private static readonly TimeSpan CharacteristicSearchTimeout = TimeSpan.FromSeconds(1);

    public async Task<IBleCharacteristic> GetCharacteristicAsync(
        string characteristicId,
        CancellationToken ct
    )
    {
        GattCharacteristic? characteristic;
        do
        {
            characteristic = (GattCharacteristic?)
                await service
                    .GetCharacteristicAsync(characteristicId)
                    .WaitAsync(CharacteristicSearchTimeout, ct);
        } while (characteristic == null);
        return new BleCharacteristic(characteristic);
    }
}
