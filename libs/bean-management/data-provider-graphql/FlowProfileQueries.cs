using MicraPro.Auth.DataDefinition;
using MicraPro.BeanManagement.DataDefinition;
using MicraPro.BeanManagement.DataProviderGraphQl.Types;

namespace MicraPro.BeanManagement.DataProviderGraphQl;

[QueryType]
public static class FlowProfileQueries
{
    [RequiredPermissions([Permission.ReadFlowProfiles])]
    public static async Task<List<FlowProfileApi>> GetFlowProfiles(
        [Service] IFlowProfileService flowProfileService,
        CancellationToken ct
    ) => (await flowProfileService.GetFlowProfilesAsync(ct)).Select(r => r.ToApi()).ToList();
}
