using System.Runtime.InteropServices;
using MicraPro.Shared.Domain;
using MicraPro.Shared.UtilsDotnet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.Shared.Infrastructure;

public static class ConfigureExtensions
{
    public static IServiceCollection AddSharedInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configurationManager
    )
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            services.AddSingleton<ISystemService, SystemService>();
        else
            services.AddSingleton<ISystemService, SystemServiceDummy>();
        return services
            .Configure<SharedInfrastructureOptions>(
                configurationManager.GetSection(SharedInfrastructureOptions.SectionName)
            )
            .AddDbContextAndMigrationService<SharedDbContext>()
            .AddScoped<IConfigurationRepository, ConfigurationRepository>();
    }
}
