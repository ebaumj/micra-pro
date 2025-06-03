namespace MicraPro.ScaleManagement.Domain.BluetoothAccess;

public interface IBleCharacteristic
{
    Guid CharacteristicId { get; }
    Task SendCommandAsync(byte[] data, CancellationToken ct);
    Task<IObservable<byte[]>> GetValueObservableAsync(CancellationToken ct);
}
