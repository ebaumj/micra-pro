using MicraPro.BrewByWeight.DataDefinition.ValueObjects;

namespace MicraPro.BrewByWeight.DataDefinition;

public interface IBrewProcess
{
    Guid ProcessId { get; }
    IObservable<BrewByWeightTracking> State { get; }
    Guid BeanId { get; }
    double InCupQuantity { get; }
    double GrindSetting { get; }
    double CoffeeQuantity { get; }
    TimeSpan TargetExtractionTime { get; }
    IBrewByWeightService.Spout Spout { get; }
}
