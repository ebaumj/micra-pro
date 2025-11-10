using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using MicraPro.ScaleManagement.DataDefinition;

namespace MicraPro.ScaleManagement.DataProviderGraphQl;

[QueryType]
public static class ScaleManagementQueries
{
    public static async Task<bool> GetScale(
        [Service] IScaleService scaleService,
        CancellationToken ct
    ) => await scaleService.GetScaleAsync(ct) != null;

    public static Task<bool> GetScanResultsAvailable(CancellationToken _) => Task.FromResult(true);

    public static Task<bool> GetIsScanning(
        [Service] IScaleService scaleService,
        CancellationToken ct
    ) => scaleService.IsScanning.FirstAsync().ToTask(ct);
}
