using MicraPro.BrewByWeight.DataDefinition;
using MicraPro.BrewByWeight.Domain.Interfaces;

namespace MicraPro.BrewByWeight.Domain.Services;

public class RetentionService : IRetentionService
{
    public Task<double> CalculateRetentionWeightAsync(
        Guid beanId,
        double inCupQuantity,
        double grindSetting,
        double coffeeQuantity,
        TimeSpan targetExtractionTime,
        IBrewByWeightService.Spout spout,
        CancellationToken ct
    )
    {
        return Task.FromResult<double>(
            spout switch
            {
                IBrewByWeightService.Spout.Single => 5,
                IBrewByWeightService.Spout.Double => 5,
                _ => 1,
            }
        );
    }
}
