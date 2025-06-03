using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.ScaleManagement.DataProviderGraphQl;

public static class ConfigureExtensions
{
    public static IRequestExecutorBuilder AddScaleManagementDataProviderGraphQlTypes(
        this IRequestExecutorBuilder builder
    )
    {
        return builder.AddDataProviderGraphQlTypes().ModifyOptions(o => o.EnableOneOf = true);
    }
}
