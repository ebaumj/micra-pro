using MicraPro.BrewByWeight.DataDefinition;
using MicraPro.BrewByWeight.DataDefinition.ValueObjects;
using MicraPro.BrewByWeight.Domain.StorageAccess;

namespace MicraPro.BrewByWeight.Domain.Interfaces;

public interface IBrewByWeightDbService
{
    Task StoreProcessAsync(
        Guid beanId,
        Guid scaleId,
        double inCupQuantity,
        double grindSetting,
        double coffeeQuantity,
        TimeSpan targetExtractionTime,
        IBrewByWeightService.Spout spout,
        IReadOnlyCollection<BrewByWeightTracking> tracking,
        CancellationToken ct
    );

    Task<IEnumerable<FinishedProcessDb>> GetFinishedAsync(CancellationToken ct);

    Task<IEnumerable<ProcessRuntimeDataDb>> GetRuntimeDataAsync(
        Guid processId,
        CancellationToken ct
    );
}
