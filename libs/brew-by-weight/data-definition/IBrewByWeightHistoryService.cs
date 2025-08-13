using MicraPro.BrewByWeight.DataDefinition.ValueObjects;

namespace MicraPro.BrewByWeight.DataDefinition;

public interface IBrewByWeightHistoryService
{
    Task<IEnumerable<BrewByWeightHistoryEntry>> ReadHistoryAsync(CancellationToken ct);
    Task<Guid> RemoveFromHistoryAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<BrewByWeightHistoryEntry>> CleanupHistoryAsync(
        bool keepLatestDistinctByProcessInputs,
        CancellationToken ct
    );
}
