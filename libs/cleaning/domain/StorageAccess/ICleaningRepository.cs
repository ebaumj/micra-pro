using MicraPro.Cleaning.DataDefinition.ValueObjects;

namespace MicraPro.Cleaning.Domain.StorageAccess;

public interface ICleaningRepository
{
    Task<CleaningCycle[]> GetCleaningSequenceAsync(CancellationToken ct);
    Task SetCleaningSequenceAsync(CleaningCycle[] sequence, CancellationToken ct);
    Task<TimeSpan> GetCleaningIntervalAsync(CancellationToken ct);
    Task SetCleaningIntervalAsync(TimeSpan interval, CancellationToken ct);
    Task<DateTime> GetLastCleaningTimeAsync(CancellationToken ct);
    Task SetLastCleaningTimeAsync(DateTime time, CancellationToken ct);
    Task<bool> IsCleaningSetAsync(CancellationToken ct);
}
