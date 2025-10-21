using MicraPro.BeanManagement.DataDefinition;

namespace MicraPro.BeanManagement.DataProviderGraphQl;

[QueryType]
public static class GrinderSettingsQueries
{
    public static async Task<double> GetGrinderOffset(
        [Service] IGrinderSettings gridSettings,
        CancellationToken ct
    ) => await gridSettings.GetGrinderOffset(ct);
}
