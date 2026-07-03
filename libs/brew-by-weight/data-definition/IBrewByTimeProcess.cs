using MicraPro.BrewByWeight.DataDefinition.ValueObjects;

namespace MicraPro.BrewByWeight.DataDefinition;

public interface IBrewByTimeProcess
{
    Guid ProcessId { get; }
    IObservable<BrewByTimeTracking> State { get; }
    TimeSpan ExtractionTime { get; }
}
