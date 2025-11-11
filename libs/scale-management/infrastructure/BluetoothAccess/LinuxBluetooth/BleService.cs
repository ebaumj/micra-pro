using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using SharedBluetooth = MicraPro.Shared.Domain.BluetoothAccess;

namespace MicraPro.ScaleManagement.Infrastructure.BluetoothAccess.LinuxBluetooth;

public class BleService(SharedBluetooth.IBleService service) : IBleService
{
    public async Task<IBleCharacteristic> GetCharacteristicAsync(
        string characteristicId,
        CancellationToken ct
    ) => new BleCharacteristic(await service.GetCharacteristicAsync(characteristicId, ct));
}
