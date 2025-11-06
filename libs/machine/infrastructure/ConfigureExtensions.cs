using System.Runtime.InteropServices;
using MicraPro.Machine.Domain.BluetoothAccess;
using MicraPro.Machine.Domain.DatabaseAccess;
using MicraPro.Machine.Infrastructure.BluetoothAccess;
using MicraPro.Machine.Infrastructure.DatabaseAccess;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.Machine.Infrastructure;

public static class ConfigureExtensions
{
    public static IServiceCollection AddMachineInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configurationManager
    )
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            services.AddTransient<IBluetoothService, BluetoothService>();
        else
            services.AddSingleton<IBluetoothService, DummyBluetoothService>();
        return services.AddScoped<IMachineRepository, MachineRepository>();
    }
}
