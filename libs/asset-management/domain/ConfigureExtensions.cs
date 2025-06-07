using MicraPro.AssetManagement.DataDefinition;
using MicraPro.AssetManagement.Domain.Interfaces;
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
            .AddSingleton<AssetStateService>()
            .AddSingleton<IPollAssetService, PollAssetService>()
            .AddScoped<AssetService>()
            .AddScoped<IAssetManagementService>(sp => sp.GetRequiredService<AssetService>())
            .AddScoped<IAssetService>(sp => sp.GetRequiredService<AssetService>())
            .AddScoped<IAssetCleaner, AssetCleaner>();
    }
}
