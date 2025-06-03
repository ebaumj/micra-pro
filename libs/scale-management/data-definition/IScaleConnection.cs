using MicraPro.ScaleManagement.DataDefinition.ValueObjects;

namespace MicraPro.ScaleManagement.DataDefinition;

public interface IScaleConnection
{
    Task Disconnect(CancellationToken ct);
    Task Tare(CancellationToken ct);
    IObservable<ScaleDataPoint> Data { get; }
}
