using MicraPro.BrewByWeight.DataDefinition;

namespace MicraPro.BrewByWeight.DataProviderGraphQl;

[MutationType]
public static class BrewByWeightMutations
{
    public static Task<Guid> StartBrewProcess(
        [Service] IBrewByWeightService brewByWeightService,
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
        return Task.FromResult(process.ProcessId);
    }

    public static async Task<Guid> StopBrewProcess(
        [Service] IBrewByWeightService brewByWeightService,
        [Service] IBrewByTimeService brewByTimeService,
        Guid processId,
        CancellationToken ct
    )
    {
        await brewByWeightService.StopBrewProcess(processId);
        await brewByTimeService.StopBrewProcess(processId);
        return processId;
    }

    public static Task<Guid> StartBrewByTimeProcess(
        [Service] IBrewByTimeService brewByTimeService,
        [Service] IBrewProcessService processService,
        TimeSpan targetTime,
        CancellationToken ct
    )
    {
        if (processService.IsBrewProcessRunning)
            throw new InvalidOperationException();
        var process = brewByTimeService.RunBrewByTime(targetTime);
        return Task.FromResult(process.ProcessId);
    }
}
