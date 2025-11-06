using MicraPro.Machine.Domain.BluetoothAccess;
using MicraPro.Shared.Domain.BluetoothAccess;

namespace MicraPro.Machine.Infrastructure.BluetoothAccess;

public class BluetoothCharacteristic(IBleCharacteristic characteristic) : IBluetoothCharacteristic
{
    public async Task<string> ReadAsync(CancellationToken ct) =>
        System.Text.Encoding.Default.GetString(await characteristic.ReadValueAsync(ct));

    public Task WriteAsync(string data, CancellationToken ct) =>
        characteristic.WriteValueAsync(data.Select(c => (byte)c).ToArray(), ct);
}
