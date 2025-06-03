using MicraPro.ScaleManagement.DataDefinition;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using MicraPro.ScaleManagement.Domain.StorageAccess;

namespace MicraPro.ScaleManagement.Domain.ScaleImplementations.BookooThemisMini;

public class Scale(ScaleDb scaleDb, IBluetoothService bluetoothService) : IScale
{
    public static Guid[] RequiredServiceIds => [ServiceId];
    public Guid Id => scaleDb.Id;
    public string Name => scaleDb.Name;

    private static readonly Guid ServiceId = Guid.Parse("00000FFE-0000-1000-8000-00805F9B34FB");
    private static readonly Guid CommandCharacteristicId = Guid.Parse(
        "0000FF12-0000-1000-8000-00805F9B34FB"
    );
    private static readonly Guid WeightDataCharacteristicId = Guid.Parse(
        "0000FF11-0000-1000-8000-00805F9B34FB"
    );

    public async Task<IScaleConnection> Connect(CancellationToken ct)
    {
        var bleConnection = await bluetoothService.ConnectDeviceAsync(scaleDb.Identifier, ct);
        var scaleService = await bleConnection.GetServiceAsync(ServiceId, ct);
        return new ScaleConnection(
            await scaleService.GetCharacteristicAsync(CommandCharacteristicId, ct),
            await (
                await scaleService.GetCharacteristicAsync(WeightDataCharacteristicId, ct)
            ).GetValueObservableAsync(ct),
            bleConnection
        );
    }
}
