namespace MicraPro.AssetManagement.Domain.ValueObjects;

public record WebhookEvent(string WebhookName, object Payload, DateTime Timestamp);
