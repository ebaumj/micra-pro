using MicraPro.BeanManagement.DataDefinition;
using MicraPro.BeanManagement.DataDefinition.ValueObjects;
using MicraPro.BeanManagement.Domain.StorageAccess;
using MicraPro.BeanManagement.Domain.ValueObjects;

namespace MicraPro.BeanManagement.Domain.Services;

public class RoasteryService(IRoasteryRepository roasteryRepository) : IRoasteryService
{
    public async Task<IRoastery> AddRoasteryAsync(
        RoasteryProperties properties,
        CancellationToken ct
    )
    {
        var entity = new RoasteryDb(properties.Name, properties.Location);
        await roasteryRepository.AddAsync(entity, ct);
        await roasteryRepository.SaveAsync(ct);
        return new Roastery(entity);
    }

    public async Task<IEnumerable<IRoastery>> GetRoasteriesAsync(CancellationToken ct) =>
        (await roasteryRepository.GetAllAsync(ct)).Select(entity => new Roastery(entity));

    public async Task<IRoastery> GetRoasteryAsync(Guid roasteryId, CancellationToken ct) =>
        new Roastery(await roasteryRepository.GetByIdAsync(roasteryId, ct));

    public async Task<IRoastery> UpdateRoasteryAsync(
        Guid roasteryId,
        RoasteryProperties properties,
        CancellationToken ct
    ) =>
        new Roastery(
            await roasteryRepository.UpdateAsync(
                roasteryId,
                properties.Name,
                properties.Location,
                ct
            )
        );

    public async Task<Guid> RemoveRoasteryAsync(Guid roasteryId, CancellationToken ct)
    {
        await roasteryRepository.DeleteAsync(roasteryId, ct);
        await roasteryRepository.SaveAsync(ct);
        return roasteryId;
    }
}
