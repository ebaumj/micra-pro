using MicraPro.ScaleManagement.Domain.BluetoothAccess;

namespace MicraPro.ScaleManagement.Domain.ScaleImplementations.Acaia.OldStyle;

public class ScaleConnection(IBleCharacteristic characteristic, IBleDeviceConnection connection)
    : ScaleConnectionBase(connection)
{
    protected override byte[] Id => Const.IdOldStyle;
    protected override TimeSpan HeartbeatInterval => Const.HeartbeatIntervalOld;

    protected override Task HeartbeatAsync(CancellationToken ct) =>
        CommandCharacteristic.SendCommandAsync(Encode(Const.MessageTypeHeartBeat, [2, 0]), ct);

    protected override IBleCharacteristic CommandCharacteristic => characteristic;
    protected override IBleCharacteristic WeightCharacteristic => characteristic;
}
