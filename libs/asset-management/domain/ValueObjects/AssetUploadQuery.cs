using MicraPro.AssetManagement.DataDefinition;

namespace MicraPro.AssetManagement.Domain.ValueObjects;

public record AssetUploadQuery(Guid AssetId, string UploadPath) : IAssetUploadQuery;
