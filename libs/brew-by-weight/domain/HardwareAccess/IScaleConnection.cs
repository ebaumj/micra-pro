namespace MicraPro.BrewByWeight.Domain.HardwareAccess;

public interface IScaleConnection
{
    IObservable<ScaleDataPoint> Data { get; }
    Task TareAsync(CancellationToken ct);
    Task DisconnectAsync(CancellationToken ct);
}
