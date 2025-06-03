using MicraPro.AssetManagement.DataDefinition;
using MicraPro.AssetManagement.DataProviderGraphQl.Types;

namespace MicraPro.AssetManagement.DataProviderGraphQl;

public static class ApiTypesExtensions
{
    public static AssetApi ToApi(this IAsset asset) => new(asset.Id, asset.Path);

    public static AssetUploadQueryApi ToApi(this IAssetUploadQuery query) =>
        new(query.AssetId, query.UploadPath);
}
