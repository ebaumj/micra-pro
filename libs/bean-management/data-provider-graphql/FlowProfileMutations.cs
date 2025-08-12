using MicraPro.Auth.DataDefinition;
using MicraPro.BeanManagement.DataDefinition;
using MicraPro.BeanManagement.DataDefinition.ValueObjects;
using MicraPro.BeanManagement.DataProviderGraphQl.Types;

namespace MicraPro.BeanManagement.DataProviderGraphQl;

[MutationType]
public static class FlowProfileMutations
{
    [RequiredPermissions([Permission.WriteFLowProfiles])]
    public static async Task<FlowProfileApi> AddFlowProfile(
        [Service] IFlowProfileService flowProfileService,
        Guid recipeId,
        FlowProfileProperties properties,
        CancellationToken ct
    ) => (await flowProfileService.AddFlowProfileAsync(properties, recipeId, ct)).ToApi();

    [RequiredPermissions([Permission.WriteFLowProfiles])]
    public static async Task<Guid> RemoveFlowProfile(
        [Service] IFlowProfileService flowProfileService,
        Guid profileId,
        CancellationToken ct
    )
    {
        await flowProfileService.RemoveFlowProfileAsync(profileId, ct);
        return profileId;
    }

    [RequiredPermissions([Permission.WriteFLowProfiles])]
    public static async Task<FlowProfileApi> UpdateFlowProfile(
        [Service] IFlowProfileService flowProfileService,
        Guid profileId,
        FlowProfileProperties properties,
        CancellationToken ct
    ) => (await flowProfileService.UpdateFlowProfileAsync(profileId, properties, ct)).ToApi();
}
