namespace MicraPro.AssetManagement.DataDefinition;

public interface IAssetConsumer
{
    Task<IEnumerable<Guid>> GetAssetsAsync(CancellationToken ct);
}
