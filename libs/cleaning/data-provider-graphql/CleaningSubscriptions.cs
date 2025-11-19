using HotChocolate.Execution;
using MicraPro.Cleaning.DataDefinition;
using MicraPro.Cleaning.DataDefinition.ValueObjects;
using MicraPro.Cleaning.DataProviderGraphQl.Services;
using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.Cleaning.DataProviderGraphQl;

[SubscriptionType]
public static class CleaningSubscriptions
{
    [Subscribe(With = nameof(SubscribeToCleaningState))]
    public static CleaningState CleaningState([EventMessage] CleaningState state) => state;

    public static ValueTask<ISourceStream<CleaningState>> SubscribeToCleaningState(
        [Service] CleaningProcessContainerService service,
        CancellationToken _
    ) => ValueTask.FromResult(service.State.ToSourceStream());

    [Subscribe(With = nameof(SubscribeToIsCleaningActive))]
    public static bool IsCleaningActive([EventMessage] bool state) => state;

    public static ValueTask<ISourceStream<bool>> SubscribeToIsCleaningActive(
        [Service] ICleaningService service,
        CancellationToken _
    ) => ValueTask.FromResult(service.IsRunning.ToSourceStream());
}
