using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using MicraPro.Cleaning.DataDefinition;
using MicraPro.Cleaning.DataDefinition.ValueObjects;
using MicraPro.Cleaning.DataProviderGraphQl.Services;

namespace MicraPro.Cleaning.DataProviderGraphQl;

[QueryType]
public static class CleaningQueries
{
    public static Task<CleaningState> GetCleaningState(
        [Service] CleaningProcessContainerService service,
        CancellationToken ct
    ) => service.State.FirstAsync().ToTask(ct);

    public static Task<bool> GetIsCleaningActive(
        [Service] ICleaningService service,
        CancellationToken ct
    ) => service.IsRunning.FirstAsync().ToTask(ct);

    public static Task<CleaningCycle[]> GetCleaningSequence(
        [Service] ICleaningService service,
        CancellationToken ct
    ) => service.GetCleaningSequenceAsync(ct);

    public static Task<TimeSpan> GetCleaningInterval(
        [Service] ICleaningService service,
        CancellationToken ct
    ) => service.GetCleaningIntervalAsync(ct);

    public static Task<DateTime> GetLastCleaningTime(
        [Service] ICleaningService service,
        CancellationToken ct
    ) => service.GetLastCleaningTimeAsync(ct);
}
