using MicraPro.AssetManagement.DataDefinition;
using MicraPro.AssetManagement.DataProviderGraphQl.Types;

namespace MicraPro.AssetManagement.DataProviderGraphQl;

[QueryType]
public static class WebhookQueries
{
    public static Task<List<WebhookApi>> GetAvailableWebhooks(
        [Service] IWebhookService webhookService,
        CancellationToken _
    ) =>
        Task.FromResult(
            webhookService
                .AvailableWebhooks.Select(h => new WebhookApi(
                    h,
                    webhookService.CreateWebhookAccessLink(h)
                ))
                .ToList()
        );

    public static Task<bool> GetWebhooksAvailable(
        [Service] IWebhookService webhookService,
        CancellationToken _
    ) => Task.FromResult(webhookService.WebhooksAvailable);
}
