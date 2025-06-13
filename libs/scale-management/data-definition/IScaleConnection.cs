using MicraPro.ScaleManagement.DataDefinition.ValueObjects;

namespace MicraPro.ScaleManagement.DataDefinition;

public interface IScaleConnection
{
    Task DisconnectAsync(CancellationToken ct);
    Task TareAsync(CancellationToken ct);
    IObservable<ScaleDataPoint> Data { get; }
}
