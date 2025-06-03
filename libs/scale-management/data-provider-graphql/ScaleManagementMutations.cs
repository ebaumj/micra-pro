using MicraPro.Auth.DataDefinition;
using MicraPro.ScaleManagement.DataDefinition;
using MicraPro.ScaleManagement.DataDefinition.ValueObjects;
using MicraPro.ScaleManagement.DataProviderGraphQl.Types;

namespace MicraPro.ScaleManagement.DataProviderGraphQl;

[MutationType]
public static class ScaleManagementMutations
{
    [RequiredPermissions([Permission.WriteScales])]
    public static async Task<ScaleApi> AddScale(
        [Service] IScaleService scaleService,
        string scaleIdentifier,
        string name,
        CancellationToken ct
    ) => new(await scaleService.AddScaleAsync(name, scaleIdentifier, ct));

    [RequiredPermissions([Permission.WriteScales])]
    public static async Task<Guid> RemoveScale(
        [Service] IScaleService scaleService,
        Guid scaleId,
        CancellationToken ct
    )
    {
        await scaleService.RemoveScaleAsync(scaleId, ct);
        return scaleId;
    }

    [RequiredPermissions([Permission.WriteScales])]
    public static async Task<ScaleApi> RenameScale(
        [Service] IScaleService scaleService,
        Guid scaleId,
        string name,
        CancellationToken ct
    ) => new(await scaleService.RenameScaleAsync(scaleId, name, ct));

    [RequiredPermissions([Permission.ReadScales])]
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

    [RequiredPermissions([Permission.ReadScales])]
    public static Task<bool> StopScanning(
        [Service] ScanCancellationContainerService scanCancellationContainerService,
        CancellationToken _
    )
    {
        scanCancellationContainerService.CancelAll();
        return Task.FromResult(true);
    }
}
