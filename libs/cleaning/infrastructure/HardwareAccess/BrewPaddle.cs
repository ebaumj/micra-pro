using MicraPro.Cleaning.Domain.HardwareAccess;
using MicraPro.Shared.Domain.WiredConnections;

namespace MicraPro.Cleaning.Infrastructure.HardwareAccess;

public class BrewPaddle(IBrewPaddleAccess brewPaddleAccess) : IBrewPaddle
{
    public Task SetPaddleAsync(bool on, CancellationToken ct) =>
        brewPaddleAccess.SetBrewPaddleOnAsync(on, ct);
}
