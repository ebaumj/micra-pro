using MicraPro.Cleaning.Domain.HardwareAccess;
using MicraPro.Cleaning.Domain.StorageAccess;
using MicraPro.Cleaning.Infrastructure.HardwareAccess;
using MicraPro.Cleaning.Infrastructure.StorageAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.Cleaning.Infrastructure;

public static class ConfigureExtensions
{
    public static IServiceCollection AddCleaningInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configurationManager
    )
    {
        return services
            .AddTransient<IBrewPaddle, BrewPaddle>()
            .AddScoped<ICleaningRepository, CleaningRepository>();
    }
}
