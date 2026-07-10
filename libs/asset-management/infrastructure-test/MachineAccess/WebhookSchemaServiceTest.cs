using MicraPro.AssetManagement.Domain.MachineAccess;
using MicraPro.AssetManagement.Infrastructure.MachineAccess;
using MicraPro.AssetManagement.Infrastructure.Services;

namespace MicraPro.AssetManagement.Infrastructure.Test.MachineAccess;

public class WebhookSchemaServiceTest
{
    [Fact]
    public void FullWebhookSchemaCoverageTest()
    {
        var service = new WebhookSchemaService(
            new BrewStateConverter(),
            new CleaningStateConverter()
        );
        foreach (var name in Enum.GetValues<IWebhookSchemaService.WebhookName>())
            service.GetDefaultPayloadValue(name);
    }
}
