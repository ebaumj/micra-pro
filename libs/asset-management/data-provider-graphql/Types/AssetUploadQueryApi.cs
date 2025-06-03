namespace MicraPro.AssetManagement.DataProviderGraphQl.Types;

[GraphQLName("AssetUploadQuery")]
public record AssetUploadQueryApi(Guid AssetId, string UploadPath);
