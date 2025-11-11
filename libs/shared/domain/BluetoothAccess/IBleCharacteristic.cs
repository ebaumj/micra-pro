namespace MicraPro.Shared.Domain.BluetoothAccess;

public interface IBleCharacteristic
{
    Task WriteValueAsync(byte[] data, CancellationToken ct);
    Task<byte[]> ReadValueAsync(CancellationToken ct);
    Task<IObservable<byte[]>> GetValueObservableAsync(CancellationToken ct);
}
