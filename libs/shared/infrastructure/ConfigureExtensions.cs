using MicraPro.Shared.Domain;
using MicraPro.Shared.UtilsDotnet;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.Shared.Infrastructure;

public static class ConfigureExtensions
{
    public static IServiceCollection AddSharedInfrastructureServices(
        this IServiceCollection services
    ) =>
        services
            .AddDbContextAndMigrationService<SharedDbContext>()
            .AddScoped<IConfigurationRepository, ConfigurationRepository>();
}
