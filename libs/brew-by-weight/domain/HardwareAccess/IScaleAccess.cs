namespace MicraPro.BrewByWeight.Domain.HardwareAccess;

public interface IScaleAccess
{
    Task<IScaleConnection> ConnectScaleAsync(CancellationToken ct);
}
