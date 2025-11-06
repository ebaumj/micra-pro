namespace MicraPro.Machine.Domain.BluetoothAccess;

public interface IBluetoothCharacteristic
{
    Task<string> ReadAsync(CancellationToken ct);
    Task WriteAsync(string data, CancellationToken ct);
}
