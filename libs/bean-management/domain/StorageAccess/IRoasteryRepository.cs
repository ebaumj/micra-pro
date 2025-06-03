using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.BeanManagement.Domain.StorageAccess;

public interface IRoasteryRepository : IRepository<RoasteryDb>
{
    Task<RoasteryDb> UpdateAsync(
        Guid roasteryId,
        string name,
        string location,
        CancellationToken ct
    );
}
