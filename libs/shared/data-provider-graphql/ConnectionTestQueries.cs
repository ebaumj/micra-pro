using MicraPro.Auth.DataDefinition;

namespace MicraPro.Shared.DataProviderGraphQl;

[QueryType]
public static class ConnectionTestQueries
{
    [RequiredPermissions([Permission.TestConnection])]
    public static Task<bool> TestConnection(CancellationToken ct) => Task.FromResult(true);
}
