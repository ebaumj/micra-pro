using MicraPro.Auth.DataDefinition;
using MicraPro.ScaleManagement.DataDefinition;
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
    ) => new(await scaleService.AddScale(name, scaleIdentifier, ct));

    [RequiredPermissions([Permission.WriteScales])]
    public static async Task<Guid> RemoveScale(
        [Service] IScaleService scaleService,
        Guid scaleId,
        CancellationToken ct
    )
    {
        await scaleService.RemoveScale(scaleId, ct);
        return scaleId;
    }

    [RequiredPermissions([Permission.WriteScales])]
    public static async Task<ScaleApi> RenameScale(
        [Service] IScaleService scaleService,
        Guid scaleId,
        string name,
        CancellationToken ct
    ) => new(await scaleService.RenameScale(scaleId, name, ct));

    [RequiredPermissions([Permission.ReadScales])]
    public static async Task<string[]> ScanForScales(
        [Service] IScaleService scaleService,
        TimeSpan? maxScanTime,
        CancellationToken ct
    )
    {
        var cancellation = ct;
        if (maxScanTime != null)
        {
            var token = new CancellationTokenSource((TimeSpan)maxScanTime);
            ct.Register(token.Cancel);
            cancellation = token.Token;
        }
        return (await scaleService.Scan(cancellation)).ToArray();
    }
}
