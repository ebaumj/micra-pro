using MicraPro.SerialCommunication.Domain.HardwareAccess;
using MicraPro.SerialCommunication.Infrastructure.HardwareAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.SerialCommunication.Infrastructure;

public static class ConfigureExtensions
{
    public static IServiceCollection AddSerialCommunicationInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services
            .Configure<SerialCommunicationInfrastructureOptions>(
                configuration.GetSection(SerialCommunicationInfrastructureOptions.SectionName)
            )
            .AddSingleton<SerialDataService>()
            .AddSingleton<ISerialDataService>(sp => sp.GetRequiredService<SerialDataService>())
            .AddHostedService(sp => sp.GetRequiredService<SerialDataService>());
    }
}
