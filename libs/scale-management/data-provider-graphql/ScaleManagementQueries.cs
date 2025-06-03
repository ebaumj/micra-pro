using MicraPro.Auth.DataDefinition;
using MicraPro.ScaleManagement.DataDefinition;
using MicraPro.ScaleManagement.DataProviderGraphQl.Types;

namespace MicraPro.ScaleManagement.DataProviderGraphQl;

[QueryType]
public static class ScaleManagementQueries
{
    [RequiredPermissions([Permission.ReadScales])]
    public static async Task<ScaleApi[]> GetScales(
        [Service] IScaleService scaleService,
        CancellationToken ct
    ) => (await scaleService.GetScales(ct)).Select(s => new ScaleApi(s)).ToArray();
}
