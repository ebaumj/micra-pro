using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.BeanManagement.Domain.StorageAccess;

public interface IBeanRepository : IRepository<BeanDb>
{
    Task<BeanDb> UpdateAsync(Guid beanId, string name, string countryCode, CancellationToken ct);
}
