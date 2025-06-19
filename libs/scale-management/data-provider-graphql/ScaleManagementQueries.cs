using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using MicraPro.Auth.DataDefinition;
using MicraPro.ScaleManagement.DataDefinition;
using MicraPro.ScaleManagement.DataDefinition.ValueObjects;
using MicraPro.ScaleManagement.DataProviderGraphQl.Types;

namespace MicraPro.ScaleManagement.DataProviderGraphQl;

[QueryType]
public static class ScaleManagementQueries
{
    [RequiredPermissions([Permission.ReadScales])]
    public static async Task<ScaleApi[]> GetScales(
        [Service] IScaleService scaleService,
        CancellationToken ct
    ) => (await scaleService.GetScalesAsync(ct)).Select(s => new ScaleApi(s)).ToArray();

    [RequiredPermissions([Permission.ReadScales])]
    public static Task<bool> GetScanResultsAvailable(CancellationToken _) => Task.FromResult(true);

    [RequiredPermissions([Permission.ReadScales])]
    public static Task<bool> GetIsScanning(
        [Service] IScaleService scaleService,
        CancellationToken ct
    ) => scaleService.IsScanning.FirstAsync().ToTask(ct);
}
