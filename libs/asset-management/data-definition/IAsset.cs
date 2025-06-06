namespace MicraPro.AssetManagement.DataDefinition;

public interface IAsset
{
    Guid Id { get; }
    string Path { get; }
    bool IsAvailableLocally { get; }
    bool IsAvailableRemotely { get; }
}
