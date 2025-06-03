using MicraPro.BeanManagement.Domain.StorageAccess;
using MicraPro.BeanManagement.Infrastructure.StorageAccess;
using MicraPro.Shared.UtilsDotnet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.BeanManagement.Infrastructure;

public static class ConfigureExtensions
{
    public static IServiceCollection AddBeanManagementInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configurationManager
    )
    {
        return services
            .AddScoped<IRoasteryRepository, RoasteryRepository>()
            .AddScoped<IRecipeRepository, RecipeRepository>()
            .AddScoped<IBeanRepository, BeanRepository>()
            .AddDbContextAndMigrationService<BeanManagementDbContext>();
    }
}
