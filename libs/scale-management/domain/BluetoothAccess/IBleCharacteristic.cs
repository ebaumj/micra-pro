namespace MicraPro.ScaleManagement.Domain.BluetoothAccess;

public interface IBleCharacteristic
{
    Task SendCommandAsync(byte[] data, CancellationToken ct);
    Task<IObservable<byte[]>> GetValueObservableAsync(CancellationToken ct);
}
