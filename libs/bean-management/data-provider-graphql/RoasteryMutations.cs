using MicraPro.Auth.DataDefinition;
using MicraPro.BeanManagement.DataDefinition;
using MicraPro.BeanManagement.DataDefinition.ValueObjects;
using MicraPro.BeanManagement.DataProviderGraphQl.Types;

namespace MicraPro.BeanManagement.DataProviderGraphQl;

[MutationType]
public static class RoasteryMutations
{
    [RequiredPermissions([Permission.WriteRoasteries])]
    public static async Task<RoasteryApi> AddRoastery(
        [Service] IRoasteryService roasteryService,
        RoasteryProperties properties,
        CancellationToken ct
    ) => (await roasteryService.AddRoasteryAsync(properties, ct)).ToApi();

    [RequiredPermissions([Permission.WriteRoasteries])]
    public static async Task<Guid> RemoveRoastery(
        [Service] IRoasteryService roasteryService,
        Guid roasteryId,
        CancellationToken ct
    )
    {
        await roasteryService.RemoveRoasteryAsync(roasteryId, ct);
        return roasteryId;
    }

    [RequiredPermissions([Permission.WriteRoasteries])]
    public static async Task<RoasteryApi> UpdateRoastery(
        [Service] IRoasteryService roasteryService,
        Guid roasteryId,
        RoasteryProperties properties,
        CancellationToken ct
    ) => (await roasteryService.UpdateRoasteryAsync(roasteryId, properties, ct)).ToApi();
}
