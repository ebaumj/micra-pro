using MicraPro.Auth.DataDefinition;
using MicraPro.FlowProfiling.DataDefinition;

namespace MicraPro.FlowProfiling.DataProviderGraphQl;

[MutationType]
public static class FlowProfilingMutations
{
    [RequiredPermissions([Permission.BrewCoffee])]
    public static Task<Guid> StartFlowProfilingProcess(
        [Service] IFlowProfilingService flowProfilingService,
        [Service] FlowProfilingProcessContainerService containerService,
        double startFlow,
        IFlowProfilingService.FlowDataPoint[] dataPoints,
        CancellationToken ct
    )
    {
        var process = flowProfilingService.RunFlowProfiling(startFlow, dataPoints);
        containerService.AddFlowProfilingProcess(process);
        return Task.FromResult(process.ProcessId);
    }

    [RequiredPermissions([Permission.BrewCoffee])]
    public static Task<Guid> StopFlowProfilingProcess(
        [Service] IFlowProfilingService flowProfilingService,
        Guid processId,
        CancellationToken ct
    )
    {
        flowProfilingService.StopFlowProfilingProcess(processId);
        return Task.FromResult(processId);
    }
}
