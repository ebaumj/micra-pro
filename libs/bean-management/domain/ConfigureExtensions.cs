using MicraPro.BeanManagement.DataDefinition;
using MicraPro.BeanManagement.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.BeanManagement.Domain;

public static class ConfigureExtensions
{
    public static IServiceCollection AddBeanManagementDomainServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services
            .AddScoped<IBeanService, BeanService>()
            .AddScoped<IRoasteryService, RoasteryService>()
            .AddScoped<IRecipeService, RecipeService>()
            .AddScoped<IGrinderSettings, GrinderSettings>()
            .AddScoped<IFlowProfileService, FlowProfileService>();
    }
}
