using MicraPro.BrewByWeight.DataDefinition.ValueObjects;

namespace MicraPro.BrewByWeight.DataDefinition;

public interface IBrewByWeightService
{
    public enum Spout
    {
        Naked,
        Single,
        Double,
    }

    IObservable<BrewByWeightState> State { get; }
    IBrewProcess RunBrewByWeight(
        Guid beanId,
        double inCupQuantity,
        double grindSetting,
        double coffeeQuantity,
        TimeSpan targetExtractionTime,
        Spout spout
    );
    IBrewProcess? GetBrewProcess(Guid processId);
    Task StopBrewProcess(Guid processId);
}
