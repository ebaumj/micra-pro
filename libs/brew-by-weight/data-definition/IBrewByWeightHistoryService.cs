using MicraPro.BrewByWeight.DataDefinition.ValueObjects;

namespace MicraPro.BrewByWeight.DataDefinition;

public interface IBrewByWeightHistoryService
{
    public Task<IEnumerable<BrewByWeightHistoryEntry>> ReadHistoryAsync(CancellationToken ct);
    public Task<Guid> RemoveFromHistoryAsync(Guid id, CancellationToken ct);
    public Task<IEnumerable<BrewByWeightHistoryEntry>> CleanupHistoryAsync(
        bool keepLatestDistinctByProcessInputs,
        CancellationToken ct
    );
}
