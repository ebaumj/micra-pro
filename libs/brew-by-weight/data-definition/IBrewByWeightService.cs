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
        Guid scaleId,
        double inCupQuantity,
        double grindSetting,
        double coffeeQuantity,
        TimeSpan targetExtractionTime,
        Spout spout
    );

    Task StopBrewProcess(Guid processId);
}
