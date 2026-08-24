using System.Globalization;
using MicraPro.BeanManagement.DataDefinition;
using MicraPro.BeanManagement.Domain.StorageAccess;

namespace MicraPro.BeanManagement.Domain.Services;

public class GrinderSettings(IKeyValueStore keyValueStore) : IGrinderSettings
{
    private const string GrinderOffsetKey = "GrinderSettings.GrinderOffset";
    private const double GrinderOffsetDefault = 0;

    public async Task<double> GetGrinderOffset(CancellationToken ct)
    {
        var offset = await keyValueStore.TryGetAsync(GrinderOffsetKey, ct);
        if (
            string.IsNullOrWhiteSpace(offset)
            || !double.TryParse(
                offset,
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out var parsedOffset
            )
        )
            return GrinderOffsetDefault;
        return parsedOffset;
    }

    public Task SetGrinderOffset(double grinderOffset, CancellationToken ct) =>
        keyValueStore.AddOrUpdateAsync(
            GrinderOffsetKey,
            grinderOffset.ToString(CultureInfo.InvariantCulture),
            ct
        );
}
