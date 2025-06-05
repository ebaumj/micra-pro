using MicraPro.BeanManagement.DataDefinition.ValueObjects;

namespace MicraPro.BeanManagement.DataDefinition;

public interface IRoasteryService
{
    Task<IRoastery> AddRoasteryAsync(RoasteryProperties properties, CancellationToken ct);
    public Task<IEnumerable<IRoastery>> GetRoasteriesAsync(CancellationToken ct);
    public Task<IRoastery> GetRoasteryAsync(Guid roasteryId, CancellationToken ct);
    Task<IRoastery> UpdateRoasteryAsync(
        Guid roasteryId,
        RoasteryProperties properties,
        CancellationToken ct
    );
    Task<Guid> RemoveRoasteryAsync(Guid roasteryId, CancellationToken ct);
}
