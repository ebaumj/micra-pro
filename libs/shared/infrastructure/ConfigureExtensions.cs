using System.Runtime.InteropServices;
using MicraPro.Shared.Domain;
using MicraPro.Shared.UtilsDotnet;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.Shared.Infrastructure;

public static class ConfigureExtensions
{
    public static IServiceCollection AddSharedInfrastructureServices(
        this IServiceCollection services
    )
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            services.AddTransient<ISystemService, SystemService>();
        else
            services.AddTransient<ISystemService, SystemServiceDummy>();
        return services
            .AddDbContextAndMigrationService<SharedDbContext>()
            .AddScoped<IConfigurationRepository, ConfigurationRepository>();
    }
}
