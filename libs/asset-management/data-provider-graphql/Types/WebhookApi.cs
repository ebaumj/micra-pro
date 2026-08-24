namespace MicraPro.AssetManagement.DataProviderGraphQl.Types;

[GraphQLName("Webhook")]
public record WebhookApi(string Name, string AccessUrl);
