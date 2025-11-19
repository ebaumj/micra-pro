using MicraPro.Cleaning.DataDefinition;
using MicraPro.Cleaning.Domain.Interfaces;
using MicraPro.Cleaning.Domain.Services;
using MicraPro.Cleaning.Domain.StorageAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.Cleaning.Domain;

public static class ConfigureExtensions
{
    public static IServiceCollection AddCleaningDomainServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services
            .AddScoped<ICleaningService, CleaningService>()
            .AddTransient<ICleaningDefaultsProvider, CleaningDefaultsProvider>()
            .AddHostedService<CleaningRepositorySeed>()
            .AddSingleton<ICleaningStateService, CleaningStateService>();
    }
}
