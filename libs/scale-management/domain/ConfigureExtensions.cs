using MicraPro.ScaleManagement.DataDefinition;
using MicraPro.ScaleManagement.Domain.ScaleImplementations;
using MicraPro.ScaleManagement.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.ScaleManagement.Domain;

public static class ConfigureExtensions
{
    private static readonly IEnumerable<Type> ImplementationFactories = AppDomain
        .CurrentDomain.GetAssemblies()
        .SelectMany(a => a.GetTypes())
        .Where(t =>
            t.GetCustomAttributes(typeof(ScaleImplementationFactoryAttribute), false).Length > 0
            && t.IsAssignableTo(typeof(IScaleImplementationFactory))
        );

    public static IServiceCollection AddScaleManagementDomainServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        foreach (var f in ImplementationFactories)
        {
            services.AddTransient(f);
            services.AddTransient<IScaleImplementationFactory>(sp =>
                (IScaleImplementationFactory)sp.GetRequiredService(f)
            );
        }
        return services
            .AddScoped<IScaleService, ScaleService>()
            .AddTransient<
                IScaleImplementationCollectionService,
                ScaleImplementationCollectionService
            >()
            .AddSingleton<ScaleImplementationMemoryService>();
    }
}
