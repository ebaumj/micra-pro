namespace MicraPro.Shared.Domain.BluetoothAccess;

public interface IBleService
{
    Task<IBleCharacteristic> GetCharacteristicAsync(string characteristicId, CancellationToken ct);
}
