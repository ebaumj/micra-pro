namespace MicraPro.AssetManagement.DataDefinition;

public interface IAssetUploadQuery
{
    Guid AssetId { get; }
    string UploadPath { get; }
}
