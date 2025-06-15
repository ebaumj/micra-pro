using MicraPro.ScaleManagement.DataDefinition;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using MicraPro.ScaleManagement.Domain.StorageAccess;

namespace MicraPro.ScaleManagement.Domain.ScaleImplementations.BookooThemisMini;

public class Scale(ScaleDb scaleDb, IBluetoothService bluetoothService) : IScale
{
    public static string[] RequiredServiceIds => [ServiceId];
    public Guid Id => scaleDb.Id;
    public string Name => scaleDb.Name;

    private static readonly string ServiceId = "00000FFE-0000-1000-8000-00805F9B34FB";
    private static readonly string CommandCharacteristicId = "0000FF12-0000-1000-8000-00805F9B34FB";
    private static readonly string WeightDataCharacteristicId =
        "0000FF11-0000-1000-8000-00805F9B34FB";

    public async Task<IScaleConnection> ConnectAsync(CancellationToken ct)
    {
        var bleConnection = await bluetoothService.ConnectDeviceAsync(scaleDb.Identifier, ct);
        var bleService = await bleConnection.GetServiceAsync(ServiceId, ct);
        var c1 = await bleService.GetCharacteristicAsync(CommandCharacteristicId, ct);
        return new ScaleConnection(
            await bleService.GetCharacteristicAsync(CommandCharacteristicId, ct),
            await (
                await bleService.GetCharacteristicAsync(WeightDataCharacteristicId, ct)
            ).GetValueObservableAsync(ct),
            bleConnection
        );
    }
}
