using MicraPro.Cleaning.DataDefinition.ValueObjects;

namespace MicraPro.Cleaning.DataDefinition;

public interface ICleaningService
{
    Task<CleaningCycle[]> GetCleaningSequenceAsync(CancellationToken ct);
    Task SetCleaningSequenceAsync(CleaningCycle[] sequence, CancellationToken ct);
    Task ResetCleaningSequenceAsync(CancellationToken ct);
    Task<TimeSpan> GetCleaningIntervalAsync(CancellationToken ct);
    Task SetCleaningIntervalAsync(TimeSpan interval, CancellationToken ct);
    Task<DateTime> GetLastCleaningTimeAsync(CancellationToken ct);

    IObservable<CleaningState> StartCleaning(CancellationToken ct);
    IObservable<bool> IsRunning { get; }
}
