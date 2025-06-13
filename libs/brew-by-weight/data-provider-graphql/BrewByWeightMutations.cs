using MicraPro.Auth.DataDefinition;
using MicraPro.BrewByWeight.DataDefinition;

namespace MicraPro.BrewByWeight.DataProviderGraphQl;

[MutationType]
public static class BrewByWeightMutations
{
    [RequiredPermissions([Permission.BrewCoffee])]
    public static Task<Guid> StartBrewProcess(
        [Service] IBrewByWeightService brewByWeightService,
        [Service] BrewProcessContainerService containerService,
        Guid beanId,
        Guid scaleId,
        double inCupQuantity,
        double grindSetting,
        double coffeeQuantity,
        TimeSpan targetExtractionTime,
        IBrewByWeightService.Spout spout,
        CancellationToken ct
    )
    {
        var process = brewByWeightService.RunBrewByWeight(
            beanId,
            scaleId,
            inCupQuantity,
            grindSetting,
            coffeeQuantity,
            targetExtractionTime,
            spout
        );
        containerService.AddBrewProcess(process);
        return Task.FromResult(process.ProcessId);
    }

    [RequiredPermissions([Permission.BrewCoffee])]
    public static Task<Guid> StopBrewProcess(
        [Service] IBrewByWeightService brewByWeightService,
        Guid processId,
        CancellationToken ct
    )
    {
        brewByWeightService.StopBrewProcess(processId);
        return Task.FromResult(processId);
    }
}
