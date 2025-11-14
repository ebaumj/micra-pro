using MicraPro.Cleaning.DataDefinition;
using MicraPro.Cleaning.DataDefinition.ValueObjects;
using MicraPro.Cleaning.DataProviderGraphQl.Services;

namespace MicraPro.Cleaning.DataProviderGraphQl;

[MutationType]
public static class CleaningMutations
{
    public static Task<bool> StartCleaning(
        [Service] CleaningProcessContainerService service,
        [Service] ICleaningService cleaningService,
        CancellationToken _
    )
    {
        service.StartCleaning(cleaningService.StartCleaning);
        return Task.FromResult(true);
    }

    public static Task<bool> StopCleaning(
        [Service] CleaningProcessContainerService service,
        CancellationToken ct
    )
    {
        service.StopCleaning();
        return Task.FromResult(true);
    }

    public static async Task<CleaningCycle[]> SetCleaningSequence(
        [Service] ICleaningService service,
        CleaningCycle[] sequence,
        CancellationToken ct
    )
    {
        await service.SetCleaningSequenceAsync(sequence, ct);
        return sequence;
    }

    public static async Task<CleaningCycle[]> ResetCleaningSequence(
        [Service] ICleaningService service,
        CancellationToken ct
    )
    {
        await service.ResetCleaningSequenceAsync(ct);
        return await service.GetCleaningSequenceAsync(ct);
    }

    public static async Task<TimeSpan> SetCleaningInterval(
        [Service] ICleaningService service,
        TimeSpan interval,
        CancellationToken ct
    )
    {
        await service.SetCleaningIntervalAsync(interval, ct);
        return interval;
    }
}
