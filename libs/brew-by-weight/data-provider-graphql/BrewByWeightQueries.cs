using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using MicraPro.Auth.DataDefinition;
using MicraPro.BrewByWeight.DataDefinition.ValueObjects;

namespace MicraPro.BrewByWeight.DataProviderGraphQl;

[QueryType]
public static class BrewByWeightQueries
{
    [RequiredPermissions([Permission.BrewCoffee])]
    public static async Task<BrewByWeightTracking?> GetBrewState(
        [Service] BrewProcessContainerService containerService,
        Guid processId,
        CancellationToken ct
    )
    {
        try
        {
            return await containerService.GetTracker(processId).FirstAsync().ToTask(ct);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
