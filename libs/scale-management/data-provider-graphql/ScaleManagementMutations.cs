using MicraPro.ScaleManagement.DataDefinition;

namespace MicraPro.ScaleManagement.DataProviderGraphQl;

[MutationType]
public static class ScaleManagementMutations
{
    public static async Task<bool> AddOrUpdateScale(
        [Service] IScaleService scaleService,
        string scaleIdentifier,
        CancellationToken ct
    )
    {
        await scaleService.AddOrUpdateScaleAsync(scaleIdentifier, ct);
        return true;
    }

    public static async Task<bool> RemoveScale(
        [Service] IScaleService scaleService,
        CancellationToken ct
    )
    {
        await scaleService.RemoveScaleAsync(ct);
        return true;
    }

    public static async Task<bool> ScanForScales(
        [Service] IScaleService scaleService,
        [Service] ScanCancellationContainerService scanCancellationContainerService,
        TimeSpan? maxScanTime,
        CancellationToken _
    )
    {
        var token = new CancellationTokenSource();
        var timeout = maxScanTime.GetValueOrDefault(TimeSpan.FromSeconds(30));
        scanCancellationContainerService.AddCancellationToken(token, timeout);
        await scaleService.ScanAsync(timeout, token.Token);
        return true;
    }

    public static Task<bool> StopScanning(
        [Service] ScanCancellationContainerService scanCancellationContainerService,
        CancellationToken _
    )
    {
        scanCancellationContainerService.CancelAll();
        return Task.FromResult(true);
    }
}
