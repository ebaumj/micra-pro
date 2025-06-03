using MicraPro.ScaleManagement.Domain.StorageAccess;
using MicraPro.Shared.UtilsDotnet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace MicraPro.ScaleManagement.Infrastructure.StorageAccess;

internal class ScaleRepository(MigratedContextProvider<ScaleManagementDbContext> contextProvider)
    : BaseSqliteRepository<ScaleDb>,
        IScaleRespository
{
    protected override async Task<DbSet<ScaleDb>> GetEntitiesAsync(CancellationToken ct) =>
        (await contextProvider.GetContextAsync(ct)).ScaleEntries;

    protected override async Task<DbContext> GetContextAsync(CancellationToken ct) =>
        await contextProvider.GetContextAsync(ct);

    public async Task<ScaleDb> UpdateNameAsync(Guid scaleId, string name, CancellationToken ct)
    {
        var entity = await GetByIdAsync(scaleId, ct);
        entity.Name = name;
        await SaveAsync(ct);
        return entity;
    }
}
