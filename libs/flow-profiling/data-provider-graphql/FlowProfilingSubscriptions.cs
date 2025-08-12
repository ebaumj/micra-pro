using HotChocolate.Execution;
using MicraPro.FlowProfiling.DataDefinition.ValueObjects;
using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.FlowProfiling.DataProviderGraphQl;

[SubscriptionType]
public static class FlowProfilingSubscriptions
{
    [Subscribe(With = nameof(SubscribeToFlowProfileState))]
    public static FlowProfileTracking FlowProfileState([EventMessage] FlowProfileTracking state) =>
        state;

    public static ValueTask<ISourceStream<FlowProfileTracking>> SubscribeToFlowProfileState(
        [Service] FlowProfilingProcessContainerService containerService,
        Guid processId,
        CancellationToken _
    ) => ValueTask.FromResult(containerService.GetTracker(processId).ToSourceStream());
}
