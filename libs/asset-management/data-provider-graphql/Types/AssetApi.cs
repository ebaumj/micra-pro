namespace MicraPro.AssetManagement.DataProviderGraphQl.Types;

[GraphQLName("Asset")]
public record AssetApi(Guid Id, string Path);
