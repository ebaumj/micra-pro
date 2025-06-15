using MicraPro.BrewByWeight.DataDefinition;
using MicraPro.BrewByWeight.Domain.Interfaces;
using MicraPro.BrewByWeight.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.BrewByWeight.Domain;

public static class ConfigureExtensions
{
    public static IServiceCollection AddBrewByWeightDomainServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services
            .AddSingleton<IBrewByWeightService, BrewByWeightService>()
            .AddSingleton<IRetentionService, RetentionService>();
    }
}
