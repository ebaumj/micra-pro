using MicraPro.BrewByWeight.Domain.HardwareAccess;
using MicraPro.Shared.Domain.WiredConnections;

namespace MicraPro.BrewByWeight.Infrastructure.HardwareAccess;

public class PaddleAccess(IBrewPaddleAccess brewPaddleAccess) : IPaddleAccess
{
    public Task SetBrewPaddleOnAsync(bool isOn, CancellationToken ct) =>
        brewPaddleAccess.SetBrewPaddleOnAsync(isOn, ct);
}
