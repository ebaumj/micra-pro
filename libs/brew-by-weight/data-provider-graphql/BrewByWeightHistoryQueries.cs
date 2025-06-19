using MicraPro.Auth.DataDefinition;
using MicraPro.BrewByWeight.DataDefinition;
using MicraPro.BrewByWeight.DataDefinition.ValueObjects;

namespace MicraPro.BrewByWeight.DataProviderGraphQl;

[QueryType]
public static class BrewByWeightHistoryQueries
{
    [RequiredPermissions([Permission.ReadStatistics])]
    public static async Task<List<BrewByWeightHistoryEntry>> GetBrewByWeightHistory(
        [Service] IBrewByWeightHistoryService brewByWeightHistoryService,
        CancellationToken ct
    ) => (await brewByWeightHistoryService.ReadHistoryAsync(ct)).ToList();
}
