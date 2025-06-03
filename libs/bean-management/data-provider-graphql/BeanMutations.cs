using MicraPro.Auth.DataDefinition;
using MicraPro.BeanManagement.DataDefinition;
using MicraPro.BeanManagement.DataDefinition.ValueObjects;
using MicraPro.BeanManagement.DataProviderGraphQl.Types;

namespace MicraPro.BeanManagement.DataProviderGraphQl;

[MutationType]
public static class BeanMutations
{
    [RequiredPermissions([Permission.WriteBeans])]
    public static async Task<BeanApi> AddBean(
        [Service] IBeanService beanService,
        Guid roasteryId,
        BeanProperties properties,
        CancellationToken ct
    ) => (await beanService.AddBeanAsync(properties, roasteryId, ct)).ToApi();

    [RequiredPermissions([Permission.WriteBeans])]
    public static async Task<Guid> RemoveBean(
        [Service] IBeanService beanService,
        Guid beanId,
        CancellationToken ct
    )
    {
        await beanService.RemoveBeanAsync(beanId, ct);
        return beanId;
    }

    [RequiredPermissions([Permission.WriteBeans])]
    public static async Task<BeanApi> UpdateBean(
        [Service] IBeanService beanService,
        Guid beanId,
        BeanProperties properties,
        CancellationToken ct
    ) => (await beanService.UpdateBeanAsync(beanId, properties, ct)).ToApi();
}
