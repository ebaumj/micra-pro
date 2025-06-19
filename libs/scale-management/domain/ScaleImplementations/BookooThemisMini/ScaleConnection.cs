using System.Reactive.Linq;
using MicraPro.ScaleManagement.DataDefinition;
using MicraPro.ScaleManagement.DataDefinition.ValueObjects;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.ScaleManagement.Domain.ScaleImplementations.BookooThemisMini;

public class ScaleConnection(
    IBleCharacteristic commandCharacteristic,
    IObservable<byte[]> weightDataCharacteristicObservable,
    IBleDeviceConnection connection
) : IScaleConnection
{
    public async Task DisconnectAsync(CancellationToken ct)
    {
        await connection.Disconnect(ct);
    }

    public Task TareAsync(CancellationToken ct) =>
        commandCharacteristic.SendCommandAsync(new BookooScaleCommand.Tare().Serialized, ct);

    private static ScaleDataPoint? FromBookooWeightData(byte[] data)
    {
        if (data.Length != 20)
            return null;
        if (data[0] != 0x03)
            return null;
        if (data[1] != 0x0B)
            return null;
        // Checksum
        if (data.Take(19).Aggregate(0, (acc, d) => acc ^ d) != data[19])
            return null;
        int weightSymbolDataPointsRaw = data[6];
        var weightInGramsRaw = data[7] << 16 | data[8] << 8 | data[9];
        int flowRateSymbolDataPointsRaw = data[10];
        var flowRateRaw = data[11] << 8 | data[12];
        return new ScaleDataPoint(
            DateTime.Now,
            (double)(flowRateSymbolDataPointsRaw == 45 ? -flowRateRaw : flowRateRaw) / 100,
            (double)(weightSymbolDataPointsRaw == 45 ? -weightInGramsRaw : weightInGramsRaw) / 100
        );
    }

    public IObservable<ScaleDataPoint> Data =>
        weightDataCharacteristicObservable.Select(FromBookooWeightData).NotNull();
}
