using MicraPro.AssetManagement.Domain.Interfaces;
using MicraPro.AssetManagement.Domain.ValueObjects;
using MicraPro.AssetManagement.Infrastructure.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MicraPro.AssetManagement.Infrastructure.Services;

public class WebhookInvokeService(
    ITokenCreatorService tokenCreatorService,
    IHttpClientWrapperFactory clientFactory,
    IAssetServerDomainProvider domainProvider,
    IOptions<AssetManagementInfrastructureOptions> options,
    ILogger<WebhookInvokeService> logger
) : IWebhookInvokeService
{
    private const string EndpointWebhook = "webhook";
    private const string EndpointWebhookUi = "webhook";
    private const string EndpointWebhookSchema = $"{EndpointWebhook}/__schema";
    private const string TokenSubject = "WebhookInvoke";
    private const string SchemaTokenSubject = "WebhookSchema";
    public bool WebhooksAvailable => options.Value.UseWebhooks;

    public TimeSpan PublishInterval =>
        TimeSpan.FromSeconds(options.Value.WebhookPublishIntervalInSeconds);

    public async Task InvokeWebhookAsync(
        IReadOnlyCollection<WebhookEvent> events,
        CancellationToken ct
    )
    {
        if (!options.Value.UseWebhooks)
        {
            logger.LogWarning("Webhooks are not configured");
            return;
        }
        using var client = clientFactory.CreateClient(
            tokenCreatorService.GenerateWebhookAccessToken(TokenSubject)
        );
        await client.MakePostRequest(
            $"{domainProvider.AssetServerExternDomain}/api/{EndpointWebhook}",
            new { events },
            ct
        );
    }

    public async Task SendSchemaAsync(
        IReadOnlyCollection<(string WebhookName, string DefaultValue)> events,
        CancellationToken ct
    )
    {
        if (!options.Value.UseWebhooks)
        {
            logger.LogWarning("Webhooks are not configured");
            return;
        }
        using var client = clientFactory.CreateClient(
            tokenCreatorService.GenerateWebhookAccessToken(SchemaTokenSubject)
        );
        await client.MakePostRequest(
            $"{domainProvider.AssetServerExternDomain}/api/{EndpointWebhookSchema}",
            new
            {
                events = events.Select(e => new
                {
                    name = e.WebhookName,
                    defaultPayloadValue = e.DefaultValue,
                }),
            },
            ct
        );
    }

    public string CreateWebhookAccessLink(string webhookName) =>
        $"{domainProvider.AssetServerExternDomain}/{EndpointWebhookUi}/{webhookName}?token={tokenCreatorService.GenerateWebhookAccessToken(webhookName)}&refreshToken={tokenCreatorService.GenerateRefreshToken()}";
}
