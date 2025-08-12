using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using MicraPro.Auth.DataDefinition;
using MicraPro.FlowProfiling.DataDefinition;
using MicraPro.FlowProfiling.DataDefinition.ValueObjects;

namespace MicraPro.FlowProfiling.DataProviderGraphQl;

[QueryType]
public static class FlowProfilingQueries
{
    [RequiredPermissions([Permission.BrewCoffee])]
    public static async Task<FlowProfileTracking?> GetFlowProfileState(
        [Service] FlowProfilingProcessContainerService containerService,
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

    [RequiredPermissions([Permission.ReadConfiguration])]
    public static Task<bool> GetIsFlowProfilingAvailable(
        [Service] IFlowProfilingService flowProfilingService
    ) => Task.FromResult(flowProfilingService.IsAvailable);
}
