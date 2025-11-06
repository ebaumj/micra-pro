using MicraPro.Machine.DataDefinition;
using MicraPro.Machine.Domain.Interfaces;
using MicraPro.Machine.Domain.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.Machine.Domain;

public static class ConfigureExtensions
{
    public static IServiceCollection AddMachineDomainServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        return services
            .AddTransient<IMachineConnectionFactory, MachineConnectionFactory>()
            .AddTransient<IMachineFactory, MachineFactory>()
            .AddSingleton<MachineService>()
            .AddSingleton<IMachineService>(sp => sp.GetRequiredService<MachineService>())
            .AddHostedService(sp => sp.GetRequiredService<MachineService>());
    }
}
