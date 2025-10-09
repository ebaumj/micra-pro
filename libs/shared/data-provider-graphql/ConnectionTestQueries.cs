namespace MicraPro.Shared.DataProviderGraphQl;

[QueryType]
public static class ConnectionTestQueries
{
    public static Task<bool> TestConnection(CancellationToken ct) => Task.FromResult(true);
}
