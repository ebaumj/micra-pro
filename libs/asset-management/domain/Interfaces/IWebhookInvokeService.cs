using MicraPro.AssetManagement.Domain.ValueObjects;

namespace MicraPro.AssetManagement.Domain.Interfaces;

public interface IWebhookInvokeService
{
    bool WebhooksAvailable { get; }
    TimeSpan PublishInterval { get; }
    Task InvokeWebhookAsync(IReadOnlyCollection<WebhookEvent> events, CancellationToken ct);
    Task SendSchemaAsync(
        IReadOnlyCollection<(string WebhookName, string DefaultValue)> events,
        CancellationToken ct
    );
    string CreateWebhookAccessLink(string webhookName);
}
