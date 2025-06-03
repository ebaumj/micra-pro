using HotChocolate.Execution.Configuration;
using Microsoft.Extensions.Configuration;
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

    public static IServiceCollection AddScaleManagementDataProviderGraphQlServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services.AddSingleton<ScanCancellationContainerService>();
    }
}
