using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using MicraPro.ScaleManagement.DataDefinition;
using MicraPro.ScaleManagement.DataProviderGraphQl.Types;

namespace MicraPro.ScaleManagement.DataProviderGraphQl;

[QueryType]
public static class ScaleManagementQueries
{
    public static async Task<ScaleApi[]> GetScales(
        [Service] IScaleService scaleService,
        CancellationToken ct
    ) => (await scaleService.GetScalesAsync(ct)).Select(s => new ScaleApi(s)).ToArray();

    public static Task<bool> GetScanResultsAvailable(CancellationToken _) => Task.FromResult(true);

    public static Task<bool> GetIsScanning(
        [Service] IScaleService scaleService,
        CancellationToken ct
    ) => scaleService.IsScanning.FirstAsync().ToTask(ct);
}
