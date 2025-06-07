using MicraPro.AssetManagement.DataDefinition;
using MicraPro.BeanManagement.Domain.StorageAccess;
using MicraPro.Shared.UtilsDotnet;
using Microsoft.EntityFrameworkCore;

namespace MicraPro.BeanManagement.Infrastructure.StorageAccess;

public class BeanRepository(MigratedContextProvider<BeanManagementDbContext> contextProvider)
    : BaseSqliteRepository<BeanDb>,
        IBeanRepository,
        IAssetConsumer
{
    protected override async Task<DbSet<BeanDb>> GetEntitiesAsync(CancellationToken ct) =>
        (await contextProvider.GetContextAsync(ct)).BeanEntries;

    protected override async Task<DbContext> GetContextAsync(CancellationToken ct) =>
        await contextProvider.GetContextAsync(ct);

    public async Task<BeanDb> UpdateAsync(
        Guid beanId,
        string name,
        string countryCode,
        Guid assetId,
        CancellationToken ct
    )
    {
        var entity = await GetByIdAsync(beanId, ct);
        entity.Name = name;
        entity.CountryCode = countryCode;
        entity.AssetId = assetId;
        await SaveAsync(ct);
        return entity;
    }

    public async Task<IEnumerable<Guid>> GetAssetsAsync(CancellationToken ct) =>
        (await GetEntitiesAsync(ct)).Where(e => e.AssetId != Guid.Empty).Select(e => e.AssetId);
}
