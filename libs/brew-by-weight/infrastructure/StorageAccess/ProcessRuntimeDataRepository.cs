using MicraPro.BrewByWeight.Domain.StorageAccess;
using MicraPro.Shared.UtilsDotnet;
using Microsoft.EntityFrameworkCore;

namespace MicraPro.BrewByWeight.Infrastructure.StorageAccess;

public class ProcessRuntimeDataRepository(
    MigratedContextProvider<BrewByWeightDbContext> contextProvider
) : BaseSqliteRepository<ProcessRuntimeDataDb>, IProcessRuntimeDataRepository
{
    protected override async Task<DbSet<ProcessRuntimeDataDb>> GetEntitiesAsync(
        CancellationToken ct
    ) => (await contextProvider.GetContextAsync(ct)).ProcessRuntimeDataEntries;

    protected override async Task<DbContext> GetContextAsync(CancellationToken ct) =>
        await contextProvider.GetContextAsync(ct);

    public async Task AddRangeAsync(
        IReadOnlyCollection<ProcessRuntimeDataDb> entites,
        CancellationToken ct
    ) => await (await GetEntitiesAsync(ct)).AddRangeAsync(entites, ct);
}
