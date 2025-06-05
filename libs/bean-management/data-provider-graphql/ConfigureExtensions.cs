using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.BeanManagement.DataProviderGraphQl;

public static class ConfigureExtensions
{
    public static IRequestExecutorBuilder AddBeanManagementDataProviderGraphQlTypes(
        this IRequestExecutorBuilder builder
    )
    {
        return builder.AddDataProviderGraphQlTypes().ModifyOptions(o => o.EnableOneOf = true);
    }
}
