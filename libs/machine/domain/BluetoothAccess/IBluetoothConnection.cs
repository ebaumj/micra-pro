namespace MicraPro.Machine.Domain.BluetoothAccess;

public interface IBluetoothConnection
{
    IBluetoothCharacteristic Read { get; }
    IBluetoothCharacteristic Write { get; }
    IBluetoothCharacteristic Auth { get; }
    IBluetoothCharacteristic Token { get; }
    Task DisconnectAsync(CancellationToken ct);
}
