using MicraPro.ScaleManagement.Domain.BluetoothAccess;

namespace MicraPro.ScaleManagement.Domain.ScaleImplementations.Acaia.PyxisStyle;

public class ScaleConnection(
    IBleCharacteristic commandCharacteristic,
    IBleCharacteristic weightCharacteristic,
    IBleDeviceConnection connection
) : ScaleConnectionBase(connection)
{
    protected override byte[] Id => Const.IdPyxisStyle;
    protected override TimeSpan HeartbeatInterval => Const.HeartbeatIntervalPyxis;

    protected override async Task HeartbeatAsync(CancellationToken ct)
    {
        await CommandCharacteristic.SendCommandAsync(
            Encode(Const.MessageTypeIdentify, Const.IdPyxisStyle),
            ct
        );
        await CommandCharacteristic.SendCommandAsync(
            Encode(Const.MessageTypeHeartBeat, [2, 0]),
            ct
        );
    }

    protected override IBleCharacteristic CommandCharacteristic => commandCharacteristic;
    protected override IBleCharacteristic WeightCharacteristic => weightCharacteristic;
}
