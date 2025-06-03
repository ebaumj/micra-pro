using InTheHand.Bluetooth;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;

namespace MicraPro.ScaleManagement.Infrastructure.BluetoothAccess.InTheHand;

public class BleService(GattService service) : IBleService
{
    public Guid ServiceId => service.Uuid;

    public async Task<IBleCharacteristic> GetCharacteristicAsync(
        Guid characteristicId,
        CancellationToken ct
    ) =>
        new BleCharacteristic(await service.GetCharacteristicAsync(characteristicId).WaitAsync(ct));

    public async Task<IBleCharacteristic[]> GetCharacteristicsAsync(CancellationToken ct) =>
        (await service.GetCharacteristicsAsync().WaitAsync(ct))
            .Select(s => new BleCharacteristic(s))
            .Cast<IBleCharacteristic>()
            .ToArray();
}
