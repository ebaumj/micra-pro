using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using MicraPro.BrewByWeight.DataDefinition;
using MicraPro.BrewByWeight.DataDefinition.ValueObjects;

namespace MicraPro.BrewByWeight.DataProviderGraphQl;

[QueryType]
public static class BrewByWeightQueries
{
    public static async Task<BrewByWeightTracking?> GetBrewState(
        [Service] IBrewByWeightService brewByWeightService,
        Guid processId,
        CancellationToken ct
    )
    {
        try
        {
            var process =
                brewByWeightService.GetBrewProcess(processId)
                ?? throw new Exception("No Brew Process Found");
            return await process.State.FirstAsync().ToTask(ct);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public static async Task<BrewByTimeTracking?> GetBrewByTimeState(
        [Service] IBrewByTimeService brewByTimeService,
        Guid processId,
        CancellationToken ct
    )
    {
        try
        {
            var process =
                brewByTimeService.GetBrewProcess(processId)
                ?? throw new Exception("No Brew Process Found");
            return await process.State.FirstAsync().ToTask(ct);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
