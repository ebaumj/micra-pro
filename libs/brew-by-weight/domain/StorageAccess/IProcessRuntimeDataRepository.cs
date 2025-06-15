using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.BrewByWeight.Domain.StorageAccess;

public interface IProcessRuntimeDataRepository : IRepository<ProcessRuntimeDataDb>
{
    Task AddRangeAsync(IReadOnlyCollection<ProcessRuntimeDataDb> entites, CancellationToken ct);
}
