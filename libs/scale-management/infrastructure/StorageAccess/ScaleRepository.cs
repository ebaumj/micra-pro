using System.Text.Json;
using MicraPro.ScaleManagement.Domain.StorageAccess;
using MicraPro.Shared.Domain.KeyValueStore;

namespace MicraPro.ScaleManagement.Infrastructure.StorageAccess;

public class ScaleRepository(IKeyValueStoreProvider keyValueStoreProvider) : IScaleRepository
{
    private static readonly string Namespace =
        $"{typeof(ScaleRepository).Namespace!}.{nameof(ScaleRepository)}";
    private readonly IKeyValueStore _store = keyValueStoreProvider.GetKeyValueStore(Namespace);
    private const string ScaleKey = "Scale";

    public async Task<ScaleDb?> GetScaleAsync(CancellationToken ct)
    {
        var value = await _store.TryGetAsync(ScaleKey, ct);
        return value != null ? Deserialize(value) : null;
    }

    public Task AddOrUpdateScaleAsync(ScaleDb scale, CancellationToken ct) =>
        _store.AddOrUpdateAsync(ScaleKey, Serialize(scale), ct);

    public Task DeleteScaleAsync(CancellationToken ct) => _store.DeleteAsync(ScaleKey, ct);

    private string Serialize(ScaleDb scale) => JsonSerializer.Serialize(scale);

    private ScaleDb? Deserialize(string value)
    {
        try
        {
            return JsonSerializer.Deserialize<ScaleDb>(value);
        }
        catch (Exception e)
        {
            // Log Exception
            return null;
        }
    }
}
