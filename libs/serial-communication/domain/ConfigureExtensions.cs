using MicraPro.SerialCommunication.DataDefinition;
using MicraPro.SerialCommunication.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.SerialCommunication.Domain;

public static class ConfigureExtensions
{
    public static IServiceCollection AddSerialCommunicationDomainServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services
            .AddHostedService<SerialCommunicationHost>()
            .AddTransient<IMessageConverterService, MessageConverterService>()
            .AddTransient<ISerialCommunicationService, SerialCommunicationService>()
            .AddSingleton<INucleoStateService, NucleoStateService>();
    }
}
