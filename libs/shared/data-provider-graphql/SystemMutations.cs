using MicraPro.Auth.DataDefinition;
using MicraPro.Shared.Domain;

namespace MicraPro.Shared.DataProviderGraphQl;

[MutationType]
public static class SystemMutations
{
    [RequiredPermissions([Permission.SystemAccess])]
    public static Task<bool> Shutdown([Service] ISystemService service, CancellationToken ct) =>
        service.ShutdownAsync(ct);

    [RequiredPermissions([Permission.SystemAccess])]
    public static Task<bool> Reboot([Service] ISystemService service, CancellationToken ct) =>
        service.RebootAsync(ct);
}
