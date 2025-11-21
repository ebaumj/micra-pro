using MicraPro.ScaleManagement.DataDefinition;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;

namespace MicraPro.ScaleManagement.Domain.ScaleImplementations.Acaia.OldStyle;

public class Scale(string identifier, IBluetoothService bluetoothService) : IScale
{
    public static string[] RequiredServiceIds => [ServiceId];

    private const string ServiceId = "00001820-0000-1000-8000-00805f9b34fb";
    private const string CharacteristicId = "00002a80-0000-1000-8000-00805f9b34fb";

    public async Task<IScaleConnection> ConnectAsync(CancellationToken ct)
    {
        var bleConnection = await bluetoothService.ConnectDeviceAsync(identifier, ct);
        var connection = new ScaleConnection(
            await (await bleConnection.GetServiceAsync(ServiceId, ct)).GetCharacteristicAsync(
                CharacteristicId,
                ct
            ),
            bleConnection
        );
        await connection.SetupAsync(ct);
        return connection;
    }
}
