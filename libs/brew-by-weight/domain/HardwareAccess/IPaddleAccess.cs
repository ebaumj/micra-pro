namespace MicraPro.BrewByWeight.Domain.HardwareAccess;

public interface IPaddleAccess
{
    Task SetBrewPaddleOnAsync(bool isOn, CancellationToken ct);
}
