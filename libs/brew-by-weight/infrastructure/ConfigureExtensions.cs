using System.Runtime.InteropServices;
using MicraPro.BrewByWeight.Domain.HardwareAccess;
using MicraPro.BrewByWeight.Domain.StorageAccess;
using MicraPro.BrewByWeight.Infrastructure.HardwareAccess;
using MicraPro.BrewByWeight.Infrastructure.StorageAccess;
using MicraPro.Shared.UtilsDotnet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.BrewByWeight.Infrastructure;

public static class ConfigureExtensions
{
    public static IServiceCollection AddBrewByWeightInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configurationManager
    )
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            services
                .AddSingleton<PaddleAccess>()
                .AddSingleton<IPaddleAccess>(sp => sp.GetRequiredService<PaddleAccess>())
                .AddHostedService(sp => sp.GetRequiredService<PaddleAccess>());
        else
            services.AddTransient<IPaddleAccess, PaddleAccessDummy>();
        return services
            .Configure<BrewByWeightInfrastructureOptions>(
                configurationManager.GetSection(BrewByWeightInfrastructureOptions.SectionName)
            )
            .AddTransient<IScaleAccess, ScaleAccess>()
            .AddScoped<IProcessRepository, ProcessRepository>()
            .AddScoped<IProcessRuntimeDataRepository, ProcessRuntimeDataRepository>()
            .AddDbContextAndMigrationService<BrewByWeightDbContext>();
    }
}
