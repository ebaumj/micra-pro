using MicraPro.ScaleManagement.DataDefinition;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;

namespace MicraPro.ScaleManagement.Domain.ScaleImplementations.Acaia.PyxisStyle;

public class Scale(string identifier, IBluetoothService bluetoothService) : IScale
{
    public static string[] DeviceNamePrefixes => ["ACAIA", "PYXIS", "LUNAR", "PROCH", "PEARL"];

    private const string ServiceId = "49535343-fe7d-4ae5-8fa9-9fafd205e455";
    private const string CommandCharacteristicId = "49535343-8841-43f4-a8d4-ecbe34729bb3";
    private const string WeightCharacteristicId = "49535343-1e4d-4bd9-ba61-23c647249616";

    public async Task<IScaleConnection> ConnectAsync(CancellationToken ct)
    {
        var bleConnection = await bluetoothService.ConnectDeviceAsync(identifier, ct);
        await Task.Delay(TimeSpan.FromSeconds(0.5), ct);
        var service = await bleConnection.GetServiceAsync(ServiceId, ct);
        var connection = new ScaleConnection(
            await service.GetCharacteristicAsync(CommandCharacteristicId, ct),
            await service.GetCharacteristicAsync(WeightCharacteristicId, ct),
            bleConnection
        );
        await connection.SetupAsync(ct);
        return connection;
    }
}
