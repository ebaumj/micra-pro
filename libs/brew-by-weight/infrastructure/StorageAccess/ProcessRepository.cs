using MicraPro.BrewByWeight.Domain.StorageAccess;
using MicraPro.Shared.UtilsDotnet;
using Microsoft.EntityFrameworkCore;

namespace MicraPro.BrewByWeight.Infrastructure.StorageAccess;

public class ProcessRepository(MigratedContextProvider<BrewByWeightDbContext> contextProvider)
    : BaseSqliteRepository<ProcessDb>,
        IProcessRepository
{
    protected override async Task<DbSet<ProcessDb>> GetEntitiesAsync(CancellationToken ct) =>
        (await contextProvider.GetContextAsync(ct)).ProcessEntries;

    protected override async Task<DbContext> GetContextAsync(CancellationToken ct) =>
        await contextProvider.GetContextAsync(ct);
}
