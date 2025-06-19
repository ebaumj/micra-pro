using MicraPro.BrewByWeight.DataDefinition.ValueObjects;
using Microsoft.Extensions.Logging;

namespace MicraPro.BrewByWeight.Domain.StorageAccess;

public static class ProcessDbExtensions
{
    public static IEnumerable<BrewByWeightHistoryEntry> ToHistoryEntries(
        this IEnumerable<ProcessDb> range,
        IEnumerable<ProcessRuntimeDataDb> runtimeData,
        ILogger logger
    ) =>
        range
            .Select(p =>
            {
                try
                {
                    return p.ToHistoryEntry(runtimeData);
                }
                catch (NotImplementedException e)
                {
                    logger.LogCritical(
                        "Process History Type {t} is not Implemented! {e}",
                        p.GetType().Name,
                        e.Message
                    );
                    return null;
                }
            })
            .Where(p => p != null)
            .Cast<BrewByWeightHistoryEntry>();

    private static BrewByWeightHistoryRuntimeData[] ToSortedArray(
        this IEnumerable<BrewByWeightHistoryRuntimeData> data
    )
    {
        var list = data.ToList();
        list.Sort((a, b) => a.TotalTime.CompareTo(b.TotalTime));
        return list.ToArray();
    }

    private static BrewByWeightHistoryEntry ToHistoryEntry(
        this ProcessDb processDb,
        IEnumerable<ProcessRuntimeDataDb> runtimeData
    ) =>
        processDb switch
        {
            FinishedProcessDb p => new BrewByWeightHistoryEntry.ProcessFinished(
                p.Id,
                p.Timestamp,
                p.BeanId,
                p.ScaleId,
                p.InCupQuantity,
                p.GrindSetting,
                p.CoffeeQuantity,
                p.TargetExtractionTime,
                p.Spout,
                p.AverageFlow,
                p.TotalQuantity,
                runtimeData
                    .Where(d => d.ProcessId == processDb.Id)
                    .Select(r => r.ToHistoryRuntimeData())
                    .ToSortedArray(),
                p.ExtractionTime
            ),
            CancelledProcessDb p => new BrewByWeightHistoryEntry.ProcessCancelled(
                p.Id,
                p.Timestamp,
                p.BeanId,
                p.ScaleId,
                p.InCupQuantity,
                p.GrindSetting,
                p.CoffeeQuantity,
                p.TargetExtractionTime,
                p.Spout,
                p.AverageFlow,
                p.TotalQuantity,
                runtimeData
                    .Where(d => d.ProcessId == processDb.Id)
                    .Select(r => r.ToHistoryRuntimeData())
                    .ToSortedArray(),
                p.TotalTime
            ),
            FailedProcessDb p => new BrewByWeightHistoryEntry.ProcessFailed(
                p.Id,
                p.Timestamp,
                p.BeanId,
                p.ScaleId,
                p.InCupQuantity,
                p.GrindSetting,
                p.CoffeeQuantity,
                p.TargetExtractionTime,
                p.Spout,
                p.AverageFlow,
                p.TotalQuantity,
                runtimeData
                    .Where(d => d.ProcessId == processDb.Id)
                    .Select(r => r.ToHistoryRuntimeData())
                    .ToSortedArray(),
                p.TotalTime,
                p.ErrorType
            ),
            _ => throw new NotImplementedException(),
        };

    private static BrewByWeightHistoryRuntimeData ToHistoryRuntimeData(
        this ProcessRuntimeDataDb db
    ) => new(db.Flow, db.TotalQuantity, db.TotalTime);
}
