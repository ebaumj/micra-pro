using MicraPro.Auth.DataDefinition;
using MicraPro.BeanManagement.DataDefinition;
using MicraPro.BeanManagement.DataProviderGraphQl.Types;

namespace MicraPro.BeanManagement.DataProviderGraphQl;

[QueryType]
public static class BeanQueries
{
    [RequiredPermissions([Permission.ReadBeans])]
    public static async Task<List<BeanApi>> GetBeans(
        [Service] IBeanService beanService,
        CancellationToken ct
    ) => (await beanService.GetBeansAsync(ct)).Select(r => r.ToApi()).ToList();

    [RequiredPermissions([Permission.ReadBeans])]
    public static async Task<BeanApi> GetBean(
        [Service] IBeanService beanService,
        Guid beanId,
        CancellationToken ct
    ) => (await beanService.GetBeanAsync(beanId, ct)).ToApi();
}
