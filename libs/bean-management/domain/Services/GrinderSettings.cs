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
        return offset is null ? GrinderOffsetDefault : double.Parse(offset);
    }

    public Task SetGrinderOffset(double grinderOffset, CancellationToken ct) =>
        keyValueStore.AddOrUpdateAsync(GrinderOffsetKey, $"{grinderOffset}", ct);
}
