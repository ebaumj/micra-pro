using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace MicraPro.ScaleManagement.Domain.ScaleImplementations.AcaiaLunar;

public class ConnectionLifetimeData : IConnectionLifetimeData
{
    private IDisposable _heartbeatSubscription = Disposable.Empty;
    private byte[] _dataBuffer = [];
    private DateTime _lastWeightTimestamp = DateTime.MinValue;
    private double _lastWeight;
    private double[] _flowAverage = [0, 0, 0, 0];

    public void DisposeHeartbeat() => _heartbeatSubscription.Dispose();

    public void SetupHeartbeat(TimeSpan period, Func<CancellationToken, Task> action)
    {
        _heartbeatSubscription.Dispose();
        _heartbeatSubscription = Observable
            .Interval(period)
            .Select(_ => Observable.FromAsync(action))
            .Merge()
            .Subscribe();
    }

    public IConnectionLifetimeData.DataFrame? MergeData(byte[] data)
    {
        lock (_dataBuffer)
        {
            _dataBuffer = _dataBuffer.Concat(data).ToArray();
            var start = Helpers.MessageStart(_dataBuffer);
            if (start < 0)
                return null;
            _dataBuffer = _dataBuffer.Skip(start).ToArray();
            if (_dataBuffer.Length < 4)
                return null;
            var length = _dataBuffer[3] - 1;
            if (_dataBuffer.Length < 6 + length)
                return null;
            var value = new IConnectionLifetimeData.DataFrame(
                _dataBuffer[2],
                _dataBuffer.Skip(4).Take(length).ToArray(),
                _dataBuffer[4 + length],
                _dataBuffer[5 + length]
            );
            _dataBuffer = _dataBuffer.Skip(6 + length).ToArray();
            return value;
        }
    }

    public double CalculateFlow(double weight)
    {
        var now = DateTime.Now;
        var diffTime = now.Subtract(_lastWeightTimestamp).TotalSeconds;
        var diffWeight = weight - _lastWeight;
        _lastWeightTimestamp = now;
        _lastWeight = weight;
        if (diffTime is > 2 or <= 0)
            return 0;
        _flowAverage = _flowAverage.Skip(1).Append(diffWeight / diffTime).ToArray();
        return _flowAverage.Sum() / _flowAverage.Length;
    }
}
