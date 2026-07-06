using System.Reactive.Linq;
using System.Text.Json;
using MicraPro.AssetManagement.Domain.MachineAccess;
using MicraPro.AssetManagement.Infrastructure.Interfaces;
using MicraPro.BrewByWeight.DataDefinition;
using MicraPro.BrewByWeight.DataDefinition.ValueObjects;
using MicraPro.Cleaning.DataDefinition.ValueObjects;

namespace MicraPro.AssetManagement.Infrastructure.MachineAccess;

public class WebhookSchemaService(
    IBrewStateConverter brewStateConverter,
    ICleaningStateConverter cleaningStateConverter
) : IWebhookSchemaService
{
    public string GetName(IWebhookSchemaService.WebhookName webhookName) => webhookName.ToString();

    private record BrewProcessDummy : IBrewProcess
    {
        public Guid ProcessId => Guid.Empty;
        public IObservable<BrewByWeightTracking> State => Observable.Empty<BrewByWeightTracking>();
        public Guid BeanId => Guid.Parse("f05ad340-e6ce-411e-93ec-418cb94446a3");
        public double InCupQuantity => 42;
        public double GrindSetting => 17;
        public double CoffeeQuantity => 18;
        public TimeSpan TargetExtractionTime => TimeSpan.FromSeconds(25);
        public IBrewByWeightService.Spout Spout => IBrewByWeightService.Spout.Single;
    }

    private record BrewByTimeProcessDummy : IBrewByTimeProcess
    {
        public Guid ProcessId => Guid.Empty;
        public IObservable<BrewByTimeTracking> State => Observable.Empty<BrewByTimeTracking>();
        public TimeSpan ExtractionTime => TimeSpan.FromSeconds(25);
    }

    private static readonly IBrewProcess DefaultBrewByWeightProcess = new BrewProcessDummy();
    private static readonly IBrewByTimeProcess DefaultBrewByTimeProcess =
        new BrewByTimeProcessDummy();

    private static readonly CleaningCycle[] DefaultCycles =
    [
        new(TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(10)),
        new(TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(0)),
    ];

    public string GetDefaultPayloadValue(IWebhookSchemaService.WebhookName webhookName) =>
        JsonSerializer.Serialize(
            webhookName switch
            {
                IWebhookSchemaService.WebhookName.BrewByWeightStarted => brewStateConverter.Convert(
                    DefaultBrewByWeightProcess,
                    new BrewByWeightTracking.Started()
                ),
                IWebhookSchemaService.WebhookName.BrewByWeightUpdate => brewStateConverter.Convert(
                    DefaultBrewByWeightProcess,
                    new BrewByWeightTracking.Running(1.6, 19.6, TimeSpan.FromSeconds(12))
                ),
                IWebhookSchemaService.WebhookName.BrewByWeightFinished =>
                    brewStateConverter.Convert(
                        DefaultBrewByWeightProcess,
                        new BrewByWeightTracking.Finished(1.6, 42, TimeSpan.FromSeconds(25))
                    ),
                IWebhookSchemaService.WebhookName.BrewByWeightStopped => brewStateConverter.Convert(
                    DefaultBrewByWeightProcess,
                    new BrewByWeightTracking.Cancelled(1.6, 42, TimeSpan.FromSeconds(25))
                ),
                IWebhookSchemaService.WebhookName.BrewByWeightError => brewStateConverter.Convert(
                    DefaultBrewByWeightProcess,
                    new BrewByWeightTracking.Failed(
                        new BrewByWeightException.ScaleConnectionFailed(),
                        1.6,
                        42,
                        TimeSpan.FromSeconds(25)
                    )
                ),
                IWebhookSchemaService.WebhookName.BrewByTimeStarted => brewStateConverter.Convert(
                    DefaultBrewByTimeProcess,
                    new BrewByTimeTracking.Started()
                ),
                IWebhookSchemaService.WebhookName.BrewByTimeUpdate => brewStateConverter.Convert(
                    DefaultBrewByTimeProcess,
                    new BrewByTimeTracking.Running(
                        TimeSpan.FromSeconds(25),
                        TimeSpan.FromSeconds(12)
                    )
                ),
                IWebhookSchemaService.WebhookName.BrewByTimeFinished => brewStateConverter.Convert(
                    DefaultBrewByTimeProcess,
                    new BrewByTimeTracking.Finished(
                        TimeSpan.FromSeconds(25),
                        TimeSpan.FromSeconds(12)
                    )
                ),
                IWebhookSchemaService.WebhookName.BrewByTimeStopped => brewStateConverter.Convert(
                    DefaultBrewByTimeProcess,
                    new BrewByTimeTracking.Cancelled(
                        TimeSpan.FromSeconds(25),
                        TimeSpan.FromSeconds(12)
                    )
                ),
                IWebhookSchemaService.WebhookName.BrewByTimeError => brewStateConverter.Convert(
                    DefaultBrewByTimeProcess,
                    new BrewByTimeTracking.Failed(
                        new BrewByWeightException.ScaleConnectionFailed(),
                        TimeSpan.FromSeconds(25),
                        TimeSpan.FromSeconds(12)
                    )
                ),
                IWebhookSchemaService.WebhookName.CleaningStarted => cleaningStateConverter.Convert(
                    DefaultCycles,
                    new CleaningState.Started()
                ),
                IWebhookSchemaService.WebhookName.CleaningUpdate => cleaningStateConverter.Convert(
                    DefaultCycles,
                    new CleaningState.Running(TimeSpan.FromSeconds(27), 1, TimeSpan.FromSeconds(2))
                ),
                IWebhookSchemaService.WebhookName.CleaningFinished =>
                    cleaningStateConverter.Convert(
                        DefaultCycles,
                        new CleaningState.Finished(TimeSpan.FromSeconds(35), 2)
                    ),
                IWebhookSchemaService.WebhookName.CleaningStopped => cleaningStateConverter.Convert(
                    DefaultCycles,
                    new CleaningState.Cancelled(TimeSpan.FromSeconds(35), 2)
                ),
                IWebhookSchemaService.WebhookName.CleaningError => cleaningStateConverter.Convert(
                    DefaultCycles,
                    new CleaningState.Failed(TimeSpan.FromSeconds(27), 1)
                ),
                _ => throw new ArgumentOutOfRangeException(nameof(webhookName), webhookName, null),
            },
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
        );
}
