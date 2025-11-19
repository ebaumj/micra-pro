namespace MicraPro.Cleaning.Domain.HardwareAccess;

public interface IBrewPaddle
{
    Task SetPaddleAsync(bool on, CancellationToken ct);
}
