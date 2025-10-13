using MicraPro.BeanManagement.DataDefinition;

namespace MicraPro.BeanManagement.DataProviderGraphQl;

[MutationType]
public static class GrinderSettingsMutations
{
    public static async Task<double> SetGrinderOffset(
        [Service] IGrinderSettings grinderSettings,
        double grinderOffset,
        CancellationToken ct
    )
    {
        await grinderSettings.SetGrinderOffset(grinderOffset, ct);
        return grinderOffset;
    }
}
