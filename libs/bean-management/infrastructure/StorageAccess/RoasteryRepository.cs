using MicraPro.BeanManagement.Domain.StorageAccess;
using MicraPro.Shared.UtilsDotnet;
using Microsoft.EntityFrameworkCore;

namespace MicraPro.BeanManagement.Infrastructure.StorageAccess;

public class RoasteryRepository(MigratedContextProvider<BeanManagementDbContext> contextProvider)
    : BaseSqliteRepository<RoasteryDb>,
        IRoasteryRepository
{
    protected override async Task<DbSet<RoasteryDb>> GetEntitiesAsync(CancellationToken ct) =>
        (await contextProvider.GetContextAsync(ct)).RoasteryEntries;

    protected override async Task<DbContext> GetContextAsync(CancellationToken ct) =>
        await contextProvider.GetContextAsync(ct);

    public async Task<RoasteryDb> UpdateAsync(
        Guid roasteryId,
        string name,
        string location,
        CancellationToken ct
    )
    {
        var entity = await GetByIdAsync(roasteryId, ct);
        entity.Name = name;
        entity.Location = location;
        await SaveAsync(ct);
        return entity;
    }
}
