using MicraPro.BrewByWeight.DataDefinition;
using MicraPro.BrewByWeight.DataDefinition.ValueObjects;

namespace MicraPro.BrewByWeight.DataProviderGraphQl;

[MutationType]
public static class BrewByWeightHistoryMutations
{
    public static async Task<List<BrewByWeightHistoryEntry>> CleanupBrewByWeightHistory(
        [Service] IBrewByWeightHistoryService brewByWeightHistoryService,
        bool keepLatestDistinctByProcessInputs,
        CancellationToken ct
    ) =>
        (
            await brewByWeightHistoryService.CleanupHistoryAsync(
                keepLatestDistinctByProcessInputs,
                ct
            )
        ).ToList();

    public static async Task<Guid> RemoveHistoryEntry(
        [Service] IBrewByWeightHistoryService brewByWeightHistoryService,
        Guid entryId,
        CancellationToken ct
    ) => await brewByWeightHistoryService.RemoveFromHistoryAsync(entryId, ct);
}
