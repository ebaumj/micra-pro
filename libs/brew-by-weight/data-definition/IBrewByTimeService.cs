using MicraPro.BrewByWeight.DataDefinition.ValueObjects;

namespace MicraPro.BrewByWeight.DataDefinition;

public interface IBrewByTimeService
{
    IObservable<BrewByTimeState> State { get; }
    IBrewByTimeProcess RunBrewByTime(TimeSpan extractionTime);
    IBrewByTimeProcess? GetBrewProcess(Guid processId);
    Task StopBrewProcess(Guid processId);
}
