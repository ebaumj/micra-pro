using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using SharedBluetooth = MicraPro.Shared.Domain.BluetoothAccess;

namespace MicraPro.ScaleManagement.Infrastructure.BluetoothAccess.LinuxBluetooth;

public class BleCharacteristic(SharedBluetooth.IBleCharacteristic characteristic)
    : IBleCharacteristic
{
    public Task SendCommandAsync(byte[] data, CancellationToken ct) =>
        characteristic.WriteValueAsync(data, ct);

    public Task<IObservable<byte[]>> GetValueObservableAsync(CancellationToken ct) =>
        characteristic.GetValueObservableAsync(ct);
}
