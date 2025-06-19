using MicraPro.Auth.DataDefinition;
using MicraPro.BeanManagement.DataDefinition;
using MicraPro.BeanManagement.DataProviderGraphQl.Types;

namespace MicraPro.BeanManagement.DataProviderGraphQl;

[QueryType]
public static class RoasteryQueries
{
    [RequiredPermissions([Permission.ReadRoasteries])]
    public static async Task<List<RoasteryApi>> GetRoasteries(
        [Service] IRoasteryService roasteryService,
        CancellationToken ct
    ) => (await roasteryService.GetRoasteriesAsync(ct)).Select(r => r.ToApi()).ToList();
}
