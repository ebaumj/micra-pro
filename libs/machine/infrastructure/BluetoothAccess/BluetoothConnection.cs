using MicraPro.Machine.Domain.BluetoothAccess;
using MicraPro.Shared.Domain.BluetoothAccess;

namespace MicraPro.Machine.Infrastructure.BluetoothAccess;

public class BluetoothConnection(IBleDeviceConnection connection) : IBluetoothConnection
{
    private const string ReadCharacteristic = "0a0b7847-e12b-09a8-b04b-8e0922a9abab";
    private const string WriteCharacteristic = "0b0b7847-e12b-09a8-b04b-8e0922a9abab";
    private const string GetTokenCharacteristic = "0c0b7847-e12b-09a8-b04b-8e0922a9abab";
    private const string AuthCharacteristic = "0d0b7847-e12b-09a8-b04b-8e0922a9abab";

    private IBleCharacteristic? _read;
    private IBleCharacteristic? _write;
    private IBleCharacteristic? _auth;
    private IBleCharacteristic? _token;

    public IBluetoothCharacteristic Read =>
        new BluetoothCharacteristic(_read ?? throw new NullReferenceException());
    public IBluetoothCharacteristic Write =>
        new BluetoothCharacteristic(_write ?? throw new NullReferenceException());
    public IBluetoothCharacteristic Auth =>
        new BluetoothCharacteristic(_auth ?? throw new NullReferenceException());
    public IBluetoothCharacteristic Token =>
        new BluetoothCharacteristic(_token ?? throw new NullReferenceException());

    public async Task SetupAsync(CancellationToken ct)
    {
        _read = await connection.GetCharacteristicAsync(ReadCharacteristic, ct);
        _write = await connection.GetCharacteristicAsync(WriteCharacteristic, ct);
        _auth = await connection.GetCharacteristicAsync(GetTokenCharacteristic, ct);
        _token = await connection.GetCharacteristicAsync(AuthCharacteristic, ct);
    }

    public Task DisconnectAsync(CancellationToken ct) => connection.Disconnect(ct);
}
