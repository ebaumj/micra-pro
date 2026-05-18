using MicraPro.BrewByWeight.DataDefinition.ValueObjects;

namespace MicraPro.BrewByWeight.DataDefinition;

public interface IBrewByTimeService
{
    IObservable<BrewByTimeState> State { get; }

    IBrewByTimeProcess RunBrewByTime(TimeSpan extractionTime);
    Task StopBrewProcess(Guid processId);
}
