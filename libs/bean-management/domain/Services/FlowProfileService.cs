using MicraPro.BeanManagement.DataDefinition;
using MicraPro.BeanManagement.DataDefinition.ValueObjects;
using MicraPro.BeanManagement.Domain.StorageAccess;
using MicraPro.BeanManagement.Domain.ValueObjects;

namespace MicraPro.BeanManagement.Domain.Services;

public class FlowProfileService(IFlowProfileRepository flowProfileRepository) : IFlowProfileService
{
    public async Task<IFlowProfile> AddFlowProfileAsync(
        FlowProfileProperties properties,
        Guid recipeId,
        CancellationToken ct
    )
    {
        var entity = new FlowProfileDb(recipeId, properties.StartFlow, properties.FlowSettings);
        await flowProfileRepository.AddAsync(entity, ct);
        await flowProfileRepository.SaveAsync(ct);
        return new FlowProfile(entity);
    }

    public async Task<IEnumerable<IFlowProfile>> GetFlowProfilesAsync(CancellationToken ct) =>
        (await flowProfileRepository.GetAllAsync(ct)).Select(entity => new FlowProfile(entity));

    public async Task<IFlowProfile> UpdateFlowProfileAsync(
        Guid profileId,
        FlowProfileProperties properties,
        CancellationToken ct
    ) =>
        new FlowProfile(
            await flowProfileRepository.UpdateAsync(
                profileId,
                properties.StartFlow,
                properties.FlowSettings,
                ct
            )
        );

    public async Task<Guid> RemoveFlowProfileAsync(Guid profileId, CancellationToken ct)
    {
        await flowProfileRepository.DeleteAsync(profileId, ct);
        await flowProfileRepository.SaveAsync(ct);
        return profileId;
    }
}
