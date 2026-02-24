using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using MicraPro.ScaleManagement.DataDefinition;
using MicraPro.ScaleManagement.DataDefinition.ValueObjects;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using MicraPro.ScaleManagement.Domain.ScaleImplementations.Shared;

namespace MicraPro.ScaleManagement.Domain.ScaleImplementations.Acaia;

public abstract class ScaleConnectionBase(IBleDeviceConnection connection) : IScaleConnection
{
    protected abstract byte[] Id { get; }
    protected abstract TimeSpan HeartbeatInterval { get; }
    protected abstract Task HeartbeatAsync(CancellationToken ct);
    protected abstract IBleCharacteristic CommandCharacteristic { get; }
    protected abstract IBleCharacteristic WeightCharacteristic { get; }

    private record DataFrame(int Command, byte[] Payload, byte Cs1, byte Cs2);

    private IDisposable _heartbeatSubscription = Disposable.Empty;
    private IDisposable _valueSubscription = Disposable.Empty;
    private readonly Subject<ScaleDataPoint?> _dataSubject = new();
    private byte[] _dataBuffer = [];
    private FlowCalculator _flowCalculator = new();

    public Task DisconnectAsync(CancellationToken ct)
    {
        _heartbeatSubscription.Dispose();
        _valueSubscription.Dispose();
        return connection.Disconnect(ct);
    }

    public Task TareAsync(CancellationToken ct) =>
        CommandCharacteristic.SendCommandAsync(Encode(Const.MessageTypeTare, [0]), ct);

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
                _flowCalculator.CalculateFlow(weightFloat.Value),
                weightFloat.Value
            );
    }

    private ScaleDataPoint? OnDataReceived(byte[] bytes)
    {
        var data = MergeData(bytes);
        if (data == null || data.Payload.Length < 7 || data.Command != Const.CommandTypeData)
            return null;
        int? skip = data.Payload switch
        {
            [Const.MessageTypeWeightData, ..] => 1,
            [Const.MessageTypeWeightTimeData, _, _, 5, ..] => 4,
            [Const.MessageTypeWeightTareData, 0, 5, ..]
            or [Const.MessageTypeWeightTareData, 8, 5, ..] => 3,
            [Const.MessageTypeWeightTareData, 9, 7, ..]
            or [Const.MessageTypeWeightTareData, 10, 7, ..] => 7,
            _ => null,
        };
        return skip.HasValue ? DecodeWeightData(data.Payload.Skip(skip.Value).ToArray()) : null;
    }

    public IObservable<ScaleDataPoint> Data => _dataSubject.Where(v => v != null)!;

    public async Task SetupAsync(CancellationToken ct)
    {
        _valueSubscription.Dispose();
        _valueSubscription = (await WeightCharacteristic.GetValueObservableAsync(ct))
            .Select(OnDataReceived)
            .Subscribe(_dataSubject);
        await CommandCharacteristic.SendCommandAsync(Encode(Const.MessageTypeIdentify, Id), ct);
        await CommandCharacteristic.SendCommandAsync(
            Encode(
                Const.MessageTypeNotificationRequest,
                [(byte)(Const.NotificationRequest.Length + 1), .. Const.NotificationRequest]
            ),
            ct
        );
        _heartbeatSubscription.Dispose();
        _heartbeatSubscription = Observable
            .Interval(HeartbeatInterval)
            .Select(_ => Observable.FromAsync(HeartbeatAsync))
            .Merge()
            .Subscribe();
    }

    private DataFrame? MergeData(byte[] data)
    {
        lock (_dataBuffer)
        {
            _dataBuffer = _dataBuffer.Concat(data).ToArray();
            var start = -1;
            for (var i = 0; i < _dataBuffer.Length - 1; i++)
                if (_dataBuffer[i] == Const.Header1 && _dataBuffer[i + 1] == Const.Header2)
                {
                    start = i;
                    break;
                }
            if (start < 0)
                return null;
            _dataBuffer = _dataBuffer.Skip(start).ToArray();
            if (_dataBuffer.Length < 4)
                return null;
            var length = _dataBuffer[3] - 1;
            if (_dataBuffer.Length < 6 + length)
                return null;
            var value = new DataFrame(
                _dataBuffer[2],
                _dataBuffer.Skip(4).Take(length).ToArray(),
                _dataBuffer[4 + length],
                _dataBuffer[5 + length]
            );
            _dataBuffer = _dataBuffer.Skip(6 + length).ToArray();
            return value;
        }
    }

    protected static byte[] Encode(byte messageType, byte[] payload)
    {
        var cs1 = payload.Where((_, i) => i % 2 == 0).Sum(b => b) & 0xFF;
        var cs2 = payload.Where((_, i) => i % 2 == 1).Sum(b => b) & 0xFF;
        return [Const.Header1, Const.Header2, messageType, .. payload, (byte)cs1, (byte)cs2];
    }
}
