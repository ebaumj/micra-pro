using MicraPro.Shared.Domain;
using Microsoft.Extensions.Logging;

namespace MicraPro.Shared.Infrastructure;

public class SystemServiceDummy(ILogger<SystemServiceDummy> logger) : ISystemService
{
    public Task<bool> ShutdownAsync(CancellationToken ct)
    {
        logger.LogInformation("shutdown");
        return Task.FromResult(false);
    }

    public Task<bool> RebootAsync(CancellationToken ct)
    {
        logger.LogInformation("reboot");
        return Task.FromResult(false);
    }
}
