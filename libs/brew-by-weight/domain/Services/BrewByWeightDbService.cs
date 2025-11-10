using MicraPro.BrewByWeight.DataDefinition;
using MicraPro.BrewByWeight.DataDefinition.ValueObjects;
using MicraPro.BrewByWeight.Domain.Interfaces;
using MicraPro.BrewByWeight.Domain.StorageAccess;

namespace MicraPro.BrewByWeight.Domain.Services;

public class BrewByWeightDbService(
    IProcessRepository processRepository,
    IProcessRuntimeDataRepository processRuntimeDataRepository
) : IBrewByWeightDbService
{
    public async Task StoreProcessAsync(
        Guid beanId,
        double inCupQuantity,
        double grindSetting,
        double coffeeQuantity,
        TimeSpan targetExtractionTime,
        IBrewByWeightService.Spout spout,
        IReadOnlyCollection<BrewByWeightTracking> tracking,
        CancellationToken ct
    )
    {
        ProcessDb process = tracking.Last() switch
        {
            BrewByWeightTracking.Finished t => new FinishedProcessDb(
                beanId,
                inCupQuantity,
                grindSetting,
                coffeeQuantity,
                targetExtractionTime,
                spout,
                t.AverageFlow,
                t.TotalQuantity,
                t.ExtractionTime
            ),
            BrewByWeightTracking.Cancelled t => new CancelledProcessDb(
                beanId,
                inCupQuantity,
                grindSetting,
                coffeeQuantity,
                targetExtractionTime,
                spout,
                t.AverageFlow,
                t.TotalQuantity,
                t.TotalTime
            ),
            BrewByWeightTracking.Failed t => new FailedProcessDb(
                beanId,
                inCupQuantity,
                grindSetting,
                coffeeQuantity,
                targetExtractionTime,
                spout,
                t.AverageFlow,
                t.TotalQuantity,
                t.TotalTime,
                t.Exception.GetType().Name
            ),
            _ => new FailedProcessDb(
                beanId,
                inCupQuantity,
                grindSetting,
                coffeeQuantity,
                targetExtractionTime,
                spout,
                0,
                0,
                TimeSpan.Zero,
                nameof(BrewByWeightException.UnknownError)
            ),
        };
        await processRepository.AddAsync(process, ct);
        await processRuntimeDataRepository.AddRangeAsync(
            tracking
                .OfType<BrewByWeightTracking.Running>()
                .Select(d => new ProcessRuntimeDataDb(
                    process.Id,
                    d.Flow,
                    d.TotalQuantity,
                    d.TotalTime
                ))
                .ToArray(),
            ct
        );
        await processRepository.SaveAsync(ct);
        await processRuntimeDataRepository.SaveAsync(ct);
    }

    public async Task<IEnumerable<FinishedProcessDb>> GetFinishedAsync(CancellationToken ct) =>
        (await processRepository.GetAllAsync(ct)).OfType<FinishedProcessDb>();

    public async Task<IEnumerable<ProcessRuntimeDataDb>> GetRuntimeDataAsync(
        Guid processId,
        CancellationToken ct
    )
    {
        var list = (await processRuntimeDataRepository.GetAllAsync(ct))
            .Where(p => p.ProcessId == processId)
            .ToList();
        list.Sort((a, b) => a.TotalTime.CompareTo(b.TotalTime));
        return list;
    }
}
