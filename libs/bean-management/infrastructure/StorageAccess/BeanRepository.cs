using MicraPro.BeanManagement.Domain.StorageAccess;
using MicraPro.Shared.UtilsDotnet;
using Microsoft.EntityFrameworkCore;

namespace MicraPro.BeanManagement.Infrastructure.StorageAccess;

public class BeanRepository(MigratedContextProvider<BeanManagementDbContext> contextProvider)
    : BaseSqliteRepository<BeanDb>,
        IBeanRepository
{
    protected override async Task<DbSet<BeanDb>> GetEntitiesAsync(CancellationToken ct) =>
        (await contextProvider.GetContextAsync(ct)).BeanEntries;

    protected override async Task<DbContext> GetContextAsync(CancellationToken ct) =>
        await contextProvider.GetContextAsync(ct);

    public async Task<BeanDb> UpdateAsync(
        Guid beanId,
        string name,
        string countryCode,
        CancellationToken ct
    )
    {
        var entity = await GetByIdAsync(beanId, ct);
        entity.Name = name;
        entity.CountryCode = countryCode;
        await SaveAsync(ct);
        return entity;
    }
}
