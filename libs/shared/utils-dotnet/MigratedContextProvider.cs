using Microsoft.EntityFrameworkCore;

namespace MicraPro.Shared.UtilsDotnet;

public class MigratedContextProvider<TDbContext>(
    TDbContext context,
    MigrationService<TDbContext> migrationService
)
    where TDbContext : DbContext
{
    public async Task<TDbContext> GetContextAsync(CancellationToken ct)
    {
        await migrationService.MigrateAsync(ct);
        return context;
    }
}
