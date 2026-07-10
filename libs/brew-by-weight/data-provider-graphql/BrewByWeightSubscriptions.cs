using HotChocolate.Execution;
using MicraPro.BrewByWeight.DataDefinition;
using MicraPro.BrewByWeight.DataDefinition.ValueObjects;
using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.BrewByWeight.DataProviderGraphQl;

[SubscriptionType]
public static class BrewByWeightSubscriptions
{
    [Subscribe(With = nameof(SubscribeToBrewState))]
    public static BrewByWeightTracking BrewState([EventMessage] BrewByWeightTracking state) =>
        state;

    public static ValueTask<ISourceStream<BrewByWeightTracking>> SubscribeToBrewState(
        [Service] IBrewByWeightService brewByWeightService,
        Guid processId,
        CancellationToken _
    )
    {
        var process =
            brewByWeightService.GetBrewProcess(processId)
            ?? throw new Exception("No Brew Process Found");
        return ValueTask.FromResult(process.State.ToSourceStream());
    }

    [Subscribe(With = nameof(SubscribeToBrewByTimeState))]
    public static BrewByTimeTracking BrewByTimeState([EventMessage] BrewByTimeTracking state) =>
        state;

    public static ValueTask<ISourceStream<BrewByTimeTracking>> SubscribeToBrewByTimeState(
        [Service] IBrewByTimeService brewByTimeService,
        Guid processId,
        CancellationToken _
    )
    {
        var process =
            brewByTimeService.GetBrewProcess(processId)
            ?? throw new Exception("No Brew Process Found");
        return ValueTask.FromResult(process.State.ToSourceStream());
    }
}
