using System.Reactive.Disposables;
using System.Reactive.Linq;
using MicraPro.AssetManagement.Domain.Interfaces;
using MicraPro.AssetManagement.Domain.MachineAccess;
using MicraPro.AssetManagement.Domain.ValueObjects;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MicraPro.AssetManagement.Domain.Services;

public class WebhookInvokeWorker(
    IWebhookInvokeService webhookInvokeService,
    IBrewStatePublisher brewStatePublisher,
    ICleaningStatePublisher cleaningStatePublisher,
    IWebhookSchemaService webhookSchemaService,
    ILogger<WebhookInvokeWorker> logger
) : IHostedService
{
    private static readonly TimeSpan SyncInterval = TimeSpan.FromSeconds(2);

    private IDisposable _subscription = Disposable.Empty;
    private IDisposable _syncSubscription = Disposable.Empty;
    private readonly CancellationTokenSource _hookInvokeTokenSource = new();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!webhookInvokeService.WebhooksAvailable)
            return;
        await SyncUntilSuccessAsync(cancellationToken);
        _subscription = Observable
            .Merge(
                SelectEvent(
                    brewStatePublisher.BrewByWeightStarted,
                    IWebhookSchemaService.WebhookName.BrewByWeightStarted
                ),
                SelectEvent(
                    brewStatePublisher.BrewByWeightUpdate,
                    IWebhookSchemaService.WebhookName.BrewByWeightUpdate
                ),
                SelectEvent(
                    brewStatePublisher.BrewByWeightFinished,
                    IWebhookSchemaService.WebhookName.BrewByWeightFinished
                ),
                SelectEvent(
                    brewStatePublisher.BrewByWeightStopped,
                    IWebhookSchemaService.WebhookName.BrewByWeightStopped
                ),
                SelectEvent(
                    brewStatePublisher.BrewByWeightError,
                    IWebhookSchemaService.WebhookName.BrewByWeightError
                ),
                SelectEvent(
                    brewStatePublisher.BrewByTimeStarted,
                    IWebhookSchemaService.WebhookName.BrewByTimeStarted
                ),
                SelectEvent(
                    brewStatePublisher.BrewByTimeUpdate,
                    IWebhookSchemaService.WebhookName.BrewByTimeUpdate
                ),
                SelectEvent(
                    brewStatePublisher.BrewByTimeFinished,
                    IWebhookSchemaService.WebhookName.BrewByTimeFinished
                ),
                SelectEvent(
                    brewStatePublisher.BrewByTimeStopped,
                    IWebhookSchemaService.WebhookName.BrewByTimeStopped
                ),
                SelectEvent(
                    brewStatePublisher.BrewByTimeError,
                    IWebhookSchemaService.WebhookName.BrewByTimeError
                ),
                SelectEvent(
                    cleaningStatePublisher.CleaningStarted,
                    IWebhookSchemaService.WebhookName.CleaningStarted
                ),
                SelectEvent(
                    cleaningStatePublisher.CleaningUpdate,
                    IWebhookSchemaService.WebhookName.CleaningUpdate
                ),
                SelectEvent(
                    cleaningStatePublisher.CleaningFinished,
                    IWebhookSchemaService.WebhookName.CleaningFinished
                ),
                SelectEvent(
                    cleaningStatePublisher.CleaningStopped,
                    IWebhookSchemaService.WebhookName.CleaningStopped
                ),
                SelectEvent(
                    cleaningStatePublisher.CleaningError,
                    IWebhookSchemaService.WebhookName.CleaningError
                )
            )
            .Buffer(webhookInvokeService.PublishInterval)
            .Where(events => events.Count > 0)
            .Subscribe(events => InvokeWebhook(events.ToArray()));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _hookInvokeTokenSource.Cancel();
        _subscription.Dispose();
        _syncSubscription.Dispose();
        return Task.CompletedTask;
    }

    private IObservable<WebhookEvent> SelectEvent(
        IObservable<object> observable,
        IWebhookSchemaService.WebhookName webhookName
    ) =>
        observable.Select(data => new WebhookEvent(
            webhookSchemaService.GetName(webhookName),
            data,
            DateTime.Now
        ));

    private void InvokeWebhook(IReadOnlyCollection<WebhookEvent> events)
    {
        Observable
            .FromAsync(async _ =>
                await webhookInvokeService.InvokeWebhookAsync(events, _hookInvokeTokenSource.Token)
            )
            .Subscribe(_ => { }, e => logger.LogError("Failed to invoke Webhook"));
    }

    private async Task SyncUntilSuccessAsync(CancellationToken ct)
    {
        if (!await SyncSchemaAsync(ct))
            _syncSubscription = Observable
                .Interval(SyncInterval)
                .Select(_ => Observable.FromAsync(SyncSchemaAsync))
                .Merge()
                .TakeUntil(success => success)
                .Subscribe();
    }

    private async Task<bool> SyncSchemaAsync(CancellationToken ct)
    {
        try
        {
            await webhookInvokeService.SendSchemaAsync(
                Enum.GetValues<IWebhookSchemaService.WebhookName>()
                    .Select(name =>
                        (
                            webhookSchemaService.GetName(name),
                            webhookSchemaService.GetDefaultPayloadValue(name)
                        )
                    )
                    .ToArray(),
                ct
            );
            return true;
        }
        catch (Exception)
        {
            logger.LogError("Failed to send Webhook schema");
        }
        return false;
    }
}
