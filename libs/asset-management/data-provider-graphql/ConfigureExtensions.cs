using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.AssetManagement.DataProviderGraphQl;

public static class ConfigureExtensions
{
    public static IRequestExecutorBuilder AddAssetManagementDataProviderGraphQlTypes(
        this IRequestExecutorBuilder builder
    )
    {
        return builder.AddDataProviderGraphQlTypes().ModifyOptions(o => o.EnableOneOf = true);
    }
}
