using MicraPro.AssetManagement.Domain.StorageAccess;
using MicraPro.Shared.UtilsDotnet;
using Microsoft.EntityFrameworkCore;

namespace MicraPro.AssetManagement.Infrastructure.StorageAccess;

public class AssetRepository(MigratedContextProvider<AssetManagementDbContext> contextProvider)
    : BaseSqliteRepository<AssetDb>,
        IAssetRepository
{
    protected override async Task<DbSet<AssetDb>> GetEntitiesAsync(CancellationToken ct) =>
        (await contextProvider.GetContextAsync(ct)).AssetEntries;

    protected override async Task<DbContext> GetContextAsync(CancellationToken ct) =>
        await contextProvider.GetContextAsync(ct);
}
