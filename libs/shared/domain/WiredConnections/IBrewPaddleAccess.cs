namespace MicraPro.Shared.Domain.WiredConnections;

public interface IBrewPaddleAccess
{
    IObservable<bool> IsOn { get; }
    Task SetBrewPaddleOnAsync(bool isOn, CancellationToken ct);
}
