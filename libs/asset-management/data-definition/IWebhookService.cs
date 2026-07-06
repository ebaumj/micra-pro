namespace MicraPro.AssetManagement.DataDefinition;

public interface IWebhookService
{
    bool WebhooksAvailable { get; }
    IReadOnlyCollection<string> AvailableWebhooks { get; }
    string CreateWebhookAccessLink(string webhookName);
}
