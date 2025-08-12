using MicraPro.BeanManagement.DataDefinition.ValueObjects;

namespace MicraPro.BeanManagement.DataDefinition;

public interface IFlowProfileService
{
    Task<IFlowProfile> AddFlowProfileAsync(
        FlowProfileProperties properties,
        Guid recipeId,
        CancellationToken ct
    );
    Task<IEnumerable<IFlowProfile>> GetFlowProfilesAsync(CancellationToken ct);
    Task<IFlowProfile> UpdateFlowProfileAsync(
        Guid profileId,
        FlowProfileProperties properties,
        CancellationToken ct
    );
    Task<Guid> RemoveFlowProfileAsync(Guid profileId, CancellationToken ct);
}
