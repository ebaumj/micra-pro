using HotChocolate.Execution;
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
        [Service] BrewProcessContainerService containerService,
        Guid processId,
        CancellationToken _
    ) => ValueTask.FromResult(containerService.GetTracker(processId).ToSourceStream());
}
