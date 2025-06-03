namespace MicraPro.ScaleManagement.Domain.BluetoothAccess;

public interface IBleService
{
    Guid ServiceId { get; }
    Task<IBleCharacteristic> GetCharacteristicAsync(Guid characteristicId, CancellationToken ct);
    Task<IBleCharacteristic[]> GetCharacteristicsAsync(CancellationToken ct);
}
