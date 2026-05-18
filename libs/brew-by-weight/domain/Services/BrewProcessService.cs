using System.Reactive.Linq;
using MicraPro.BrewByWeight.DataDefinition;
using MicraPro.BrewByWeight.DataDefinition.ValueObjects;

namespace MicraPro.BrewByWeight.Domain.Services;

public class BrewProcessService(
    IBrewByWeightService brewByWeightService,
    IBrewByTimeService brewByTimeService
) : IBrewProcessService
{
    public bool IsBrewProcessRunning =>
        brewByWeightService.State.FirstAsync().Wait() is BrewByWeightState.Running
        || brewByTimeService.State.FirstAsync().Wait() is BrewByTimeState.Running;
}
