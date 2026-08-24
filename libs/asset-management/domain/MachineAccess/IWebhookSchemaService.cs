namespace MicraPro.AssetManagement.Domain.MachineAccess;

public interface IWebhookSchemaService
{
    public enum WebhookName
    {
        BrewByWeightStarted,
        BrewByWeightUpdate,
        BrewByWeightFinished,
        BrewByWeightStopped,
        BrewByWeightError,
        BrewByTimeStarted,
        BrewByTimeUpdate,
        BrewByTimeFinished,
        BrewByTimeStopped,
        BrewByTimeError,
        CleaningStarted,
        CleaningUpdate,
        CleaningFinished,
        CleaningStopped,
        CleaningError,
    }

    public string GetName(WebhookName webhookName);
    public string GetDefaultPayloadValue(WebhookName webhookName);
}
