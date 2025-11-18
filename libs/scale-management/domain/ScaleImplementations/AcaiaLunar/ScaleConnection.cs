using System.Reactive.Linq;
using MicraPro.ScaleManagement.DataDefinition;
using MicraPro.ScaleManagement.DataDefinition.ValueObjects;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;

namespace MicraPro.ScaleManagement.Domain.ScaleImplementations.AcaiaLunar;

public class ScaleConnection(
    IBleCharacteristic commandCharacteristic,
    IObservable<byte[]> weightDataCharacteristicObservable,
    IBleDeviceConnection connection,
    IConnectionLifetimeData connectionLifetimeData
) : IScaleConnection
{
    private static readonly TimeSpan HeartbeatInterval = TimeSpan.FromSeconds(5);

    public Task DisconnectAsync(CancellationToken ct)
    {
        connectionLifetimeData.DisposeHeartbeat();
        return connection.Disconnect(ct);
    }

    public Task TareAsync(CancellationToken ct) =>
        commandCharacteristic.SendCommandAsync(Encode(Helpers.MessageTypeTare, [0]), ct);

    private ScaleDataPoint? DecodeWeightData(byte[] data)
    {
        if (data.Length < 6)
            return null;
        var weight = data[1] << 8 | data[0];
        var unit = data[4];
        var sign = data[5];
        if ((sign & 0x02) == 0x02)
            weight = 0 - weight;
        var weightFloat = unit switch
        {
            0 => weight,
            1 => (double?)weight / 10,
            2 => (double?)weight / 100,
            3 => (double?)weight / 1000,
            4 => (double?)weight / 10000,
            _ => null,
        };
        return !weightFloat.HasValue
            ? null
            : new ScaleDataPoint(
                DateTime.Now,
                connectionLifetimeData.CalculateFlow(weightFloat.Value),
                weightFloat.Value
            );
    }

    private ScaleDataPoint? OnDataReceived(byte[] bytes)
    {
        var data = connectionLifetimeData.MergeData(bytes);
        if (data == null || data.Payload.Length < 7 || data.Command != Helpers.CommandTypeData)
            return null;
        int? skip = data.Payload switch
        {
            [Helpers.MessageTypeWeightData, ..] => 1,
            [Helpers.MessageTypeWeightTimeData, _, _, 5, ..] => 4,
            [Helpers.MessageTypeWeightTareData, 0, 5, ..]
            or [Helpers.MessageTypeWeightTareData, 8, 5, ..] => 3,
            [Helpers.MessageTypeWeightTareData, 9, 7, ..]
            or [Helpers.MessageTypeWeightTareData, 10, 7, ..] => 7,
            _ => null,
        };
        return skip.HasValue ? DecodeWeightData(data.Payload.Skip(skip.Value).ToArray()) : null;
    }

    public IObservable<ScaleDataPoint> Data =>
        weightDataCharacteristicObservable.Select(OnDataReceived).Where(v => v != null)!;

    public async Task IdendifyAsync(CancellationToken ct)
    {
        byte[] notificationRequest =
        [
            0, // weight
            1, // weight argument
            1, // battery
            2, // battery argument
            2, // timer
            5, // timer argument (number heartbeats between timer messages)
            3, // key
            4, // setting
        ];
        byte[] id =
        [
            0x2d,
            0x2d,
            0x2d,
            0x2d,
            0x2d,
            0x2d,
            0x2d,
            0x2d,
            0x2d,
            0x2d,
            0x2d,
            0x2d,
            0x2d,
            0x2d,
            0x2d,
        ];
        await commandCharacteristic.SendCommandAsync(Encode(Helpers.MessageTypeIdentify, id), ct);
        await commandCharacteristic.SendCommandAsync(
            Encode(
                Helpers.MessageTypeNotificationRequest,
                [(byte)(notificationRequest.Length + 1), .. notificationRequest]
            ),
            ct
        );
        connectionLifetimeData.SetupHeartbeat(
            HeartbeatInterval,
            c =>
                commandCharacteristic.SendCommandAsync(
                    Encode(Helpers.MessageTypeHeartBeat, [2, 0]),
                    c
                )
        );
    }

    private static byte[] Encode(byte messageType, byte[] payload)
    {
        var cs1 = payload.Where((_, i) => i % 2 == 0).Sum(b => b) & 0xFF;
        var cs2 = payload.Where((_, i) => i % 2 == 1).Sum(b => b) & 0xFF;
        return [Helpers.Header1, Helpers.Header2, messageType, .. payload, (byte)cs1, (byte)cs2];
    }
}
