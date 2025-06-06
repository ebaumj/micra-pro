using MicraPro.AssetManagement.DataDefinition;
using MicraPro.AssetManagement.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.AssetManagement.Domain;

public static class ConfigureExtensions
{
    public static IServiceCollection AddAssetManagementDomainServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services
            .AddHostedService<StartupAssetFetcher>()
            .AddScoped<IAssetService, AssetService>();
    }
}
