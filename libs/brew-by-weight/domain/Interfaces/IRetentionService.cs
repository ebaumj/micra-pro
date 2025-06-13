using MicraPro.BrewByWeight.DataDefinition;

namespace MicraPro.BrewByWeight.Domain.Interfaces;

public interface IRetentionService
{
    Task<double> CalculateRetentionWeightAsync(
        Guid beanId,
        double inCupQuantity,
        double grindSetting,
        double coffeeQuantity,
        TimeSpan targetExtractionTime,
        IBrewByWeightService.Spout spout,
        CancellationToken ct
    );
}
