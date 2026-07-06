using MicraPro.AssetManagement.DataDefinition;
using MicraPro.AssetManagement.Domain.Interfaces;
using MicraPro.AssetManagement.Domain.MachineAccess;

namespace MicraPro.AssetManagement.Domain.Services;

public class WebhookService(
    IWebhookSchemaService webhookSchemaService,
    IWebhookInvokeService webhookInvokeService
) : IWebhookService
{
    public bool WebhooksAvailable => webhookInvokeService.WebhooksAvailable;

    public IReadOnlyCollection<string> AvailableWebhooks =>
        Enum.GetValues<IWebhookSchemaService.WebhookName>()
            .Select(webhookSchemaService.GetName)
            .ToArray();

    public string CreateWebhookAccessLink(string webhookName) =>
        webhookInvokeService.CreateWebhookAccessLink(webhookName);
}
