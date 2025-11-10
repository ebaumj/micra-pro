using MicraPro.BrewByWeight.DataDefinition;
using MicraPro.BrewByWeight.DataDefinition.ValueObjects;
using MicraPro.BrewByWeight.Domain.StorageAccess;
using Microsoft.Extensions.Logging;

namespace MicraPro.BrewByWeight.Domain.Services;

public class BrewByWeightHistoryService(
    IProcessRepository processRepository,
    IProcessRuntimeDataRepository processRuntimeDataRepository,
    ILogger<BrewByWeightHistoryService> logger
) : IBrewByWeightHistoryService
{
    private record ProcessInputs(
        Guid BeanId,
        double InCupQuantity,
        double GrindSetting,
        double CoffeeQuantity,
        TimeSpan TargetExtractionTime,
        IBrewByWeightService.Spout Spout
    )
    {
        public ProcessInputs(ProcessDb db)
            : this(
                db.BeanId,
                db.InCupQuantity,
                db.GrindSetting,
                db.CoffeeQuantity,
                db.TargetExtractionTime,
                db.Spout
            ) { }
    }

    public async Task<IEnumerable<BrewByWeightHistoryEntry>> ReadHistoryAsync(
        CancellationToken ct
    ) =>
        (await processRepository.GetAllAsync(ct)).ToHistoryEntries(
            await processRuntimeDataRepository.GetAllAsync(ct),
            logger
        );

    public async Task<Guid> RemoveFromHistoryAsync(Guid id, CancellationToken ct)
    {
        var allRuntimeData = (await processRuntimeDataRepository.GetAllAsync(ct)).ToArray();
        foreach (var r in allRuntimeData.Where(r => r.ProcessId == id))
            await processRuntimeDataRepository.DeleteAsync(r.Id, ct);
        await processRepository.DeleteAsync(id, ct);
        await processRepository.SaveAsync(ct);
        return id;
    }

    public async Task<IEnumerable<BrewByWeightHistoryEntry>> CleanupHistoryAsync(
        bool keepLatestDistinctByProcessInputs,
        CancellationToken ct
    )
    {
        var allHistory = (await processRepository.GetAllAsync(ct)).ToArray();
        var allRuntimeData = (await processRuntimeDataRepository.GetAllAsync(ct)).ToArray();
        var keepIds = keepLatestDistinctByProcessInputs
            ? allHistory
                .OfType<FinishedProcessDb>()
                .Select(p => new ProcessInputs(p))
                .Distinct()
                .Select(key =>
                    allHistory.Where(p => new ProcessInputs(p) == key).MaxBy(p => p.Timestamp)
                )
                .OfType<ProcessDb>()
                .Select(latest => latest.Id)
            : [];
        foreach (var p in allHistory.Where(p => !keepIds.Contains(p.Id)))
        {
            foreach (var r in allRuntimeData.Where(r => r.ProcessId == p.Id))
                await processRuntimeDataRepository.DeleteAsync(r.Id, ct);
            await processRepository.DeleteAsync(p.Id, ct);
        }
        return allHistory
            .Where(p => keepIds.Contains(p.Id))
            .ToHistoryEntries(allRuntimeData, logger);
    }
}
