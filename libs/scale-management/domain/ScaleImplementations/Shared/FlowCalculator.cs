namespace MicraPro.ScaleManagement.Domain.ScaleImplementations.Shared;

public class FlowCalculator
{
    private const double FlowThreshold = 10;

    private DateTime _lastWeightTimestamp = DateTime.MinValue;
    private double _lastWeight;
    private double[] _flowAverage = [0, 0, 0, 0];

    public double CalculateFlow(double weight)
    {
        var now = DateTime.Now;
        var diffTime = now.Subtract(_lastWeightTimestamp).TotalSeconds;
        var diffWeight = weight - _lastWeight;
        _lastWeightTimestamp = now;
        _lastWeight = weight;
        if (diffTime is > 2 or <= 0)
            return 0;
        var flow = diffWeight / diffTime;
        if (flow < FlowThreshold)
            _flowAverage = _flowAverage.Skip(1).Append(diffWeight / diffTime).ToArray();
        return _flowAverage.Sum() / _flowAverage.Length;
    }
}
