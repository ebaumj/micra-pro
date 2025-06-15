using MicraPro.BrewByWeight.DataDefinition.ValueObjects;

namespace MicraPro.BrewByWeight.DataDefinition;

public interface IBrewProcess
{
    Guid ProcessId { get; }
    IObservable<BrewByWeightTracking> State { get; }
}
