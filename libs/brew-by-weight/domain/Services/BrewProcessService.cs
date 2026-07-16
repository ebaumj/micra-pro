using System.Reactive.Linq;
using MicraPro.BrewByWeight.DataDefinition;
using MicraPro.BrewByWeight.DataDefinition.ValueObjects;

namespace MicraPro.BrewByWeight.Domain.Services;

public class BrewProcessService(
    IBrewByWeightService brewByWeightService,
    IBrewByTimeService brewByTimeService
) : IBrewProcessService
{
    public Task<bool> IsBrewProcessRunning => GetIsBrewProcessWaiting();

    private async Task<bool> GetIsBrewProcessWaiting() =>
        await brewByWeightService.State.FirstAsync() is BrewByWeightState.Running
        || await brewByTimeService.State.FirstAsync() is BrewByTimeState.Running;
}
