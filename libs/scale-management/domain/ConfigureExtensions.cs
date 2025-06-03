using MicraPro.ScaleManagement.DataDefinition;
using MicraPro.ScaleManagement.Domain.ScaleImplementations;
using MicraPro.ScaleManagement.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.ScaleManagement.Domain;

public static class ConfigureExtensions
{
    public static IServiceCollection AddScaleManagementDomainServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services
            .AddScoped<IScaleService, ScaleService>()
            .AddSingleton<
                IScaleImplementationCollectionService,
                ScaleImplementationCollectionService
            >();
    }
}
