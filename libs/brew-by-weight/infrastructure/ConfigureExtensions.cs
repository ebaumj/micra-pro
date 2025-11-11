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
        return services
            .AddTransient<IPaddleAccess, PaddleAccess>()
            .AddTransient<IScaleAccess, ScaleAccess>()
            .AddScoped<IProcessRepository, ProcessRepository>()
            .AddScoped<IProcessRuntimeDataRepository, ProcessRuntimeDataRepository>()
            .AddDbContextAndMigrationService<BrewByWeightDbContext>();
    }
}
