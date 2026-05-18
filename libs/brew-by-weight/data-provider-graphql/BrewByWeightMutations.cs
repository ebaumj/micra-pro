using MicraPro.BrewByWeight.DataDefinition;

namespace MicraPro.BrewByWeight.DataProviderGraphQl;

[MutationType]
public static class BrewByWeightMutations
{
    public static Task<Guid> StartBrewProcess(
        [Service] IBrewByWeightService brewByWeightService,
        [Service] BrewProcessContainerService containerService,
        [Service] IBrewProcessService processService,
        Guid beanId,
        double inCupQuantity,
        double grindSetting,
        double coffeeQuantity,
        TimeSpan targetExtractionTime,
        IBrewByWeightService.Spout spout,
        CancellationToken ct
    )
    {
        if (processService.IsBrewProcessRunning)
            throw new InvalidOperationException();
        var process = brewByWeightService.RunBrewByWeight(
            beanId,
            inCupQuantity,
            grindSetting,
            coffeeQuantity,
            targetExtractionTime,
            spout
        );
        containerService.AddBrewProcess(process);
        return Task.FromResult(process.ProcessId);
    }

    public static Task<Guid> StopBrewProcess(
        [Service] IBrewByWeightService brewByWeightService,
        [Service] IBrewByTimeService brewByTimeService,
        Guid processId,
        CancellationToken ct
    )
    {
        brewByWeightService.StopBrewProcess(processId);
        brewByTimeService.StopBrewProcess(processId);
        return Task.FromResult(processId);
    }

    public static Task<Guid> StartBrewByTimeProcess(
        [Service] IBrewByTimeService brewByTimeService,
        [Service] BrewByTimeProcessContainerService containerService,
        [Service] IBrewProcessService processService,
        TimeSpan targetTime,
        CancellationToken ct
    )
    {
        if (processService.IsBrewProcessRunning)
            throw new InvalidOperationException();
        var process = brewByTimeService.RunBrewByTime(targetTime);
        containerService.AddBrewProcess(process);
        return Task.FromResult(process.ProcessId);
    }
}
