using Linux.Bluetooth;
using Linux.Bluetooth.Extensions;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.ScaleManagement.Infrastructure.BluetoothAccess.LinuxBluetooth;

public class BleService(IGattService1 service, Guid serviceId) : IBleService
{
    public Guid ServiceId => serviceId;

    public async Task<IBleCharacteristic> GetCharacteristicAsync(
        Guid characteristicId,
        CancellationToken ct
    ) =>
        await CreateCharacteristicAsync(
            await service.GetCharacteristicAsync(characteristicId.ToString()),
            ct
        );

    public async Task<IBleCharacteristic[]> GetCharacteristicsAsync(CancellationToken ct) =>
        (
            await (await service.GetCharacteristicsAsync().WaitAsync(ct)).SelectAsync(c =>
                CreateCharacteristicAsync(c, ct)
            )
        ).ToArray();

    private async Task<IBleCharacteristic> CreateCharacteristicAsync(
        IGattCharacteristic1 characteristic,
        CancellationToken ct
    ) =>
        await GetCharacteristicAsync(
            Guid.Parse(await characteristic.GetUUIDAsync().WaitAsync(ct)),
            ct
        );

    private static async Task<IBleCharacteristic> CreateCharacteristicAsync(
        GattCharacteristic characteristic,
        CancellationToken ct
    ) =>
        new BleCharacteristic(
            characteristic,
            Guid.Parse(await characteristic.GetUUIDAsync().WaitAsync(ct))
        );
}
