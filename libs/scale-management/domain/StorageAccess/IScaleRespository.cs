using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.ScaleManagement.Domain.StorageAccess;

public interface IScaleRespository : IRepository<ScaleDb>
{
    Task<ScaleDb> UpdateNameAsync(Guid scaleId, string name, CancellationToken ct);
}
