using MicraPro.ScaleManagement.Domain.StorageAccess;
using MicraPro.ScaleManagement.Infrastructure.BluetoothAccess;
using MicraPro.ScaleManagement.Infrastructure.StorageAccess;
using MicraPro.Shared.UtilsDotnet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.ScaleManagement.Infrastructure;

public static class ConfigureExtensions
{
    public static IServiceCollection AddScaleManagementInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configurationManager
    )
    {
        return services
            .Configure<ScaleManagementInfrastructureOptions>(
                configurationManager.GetSection(ScaleManagementInfrastructureOptions.SectionName)
            )
            .AddBluetoothService()
            .AddScoped<IScaleRepository, ScaleRepository>();
    }
}
