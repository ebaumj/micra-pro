namespace MicraPro.BrewByWeight.Domain.HardwareAccess;

public interface IScaleAccess
{
    Task<IScaleConnection> ConnectScaleAsync(Guid scaleId, CancellationToken ct);
}
