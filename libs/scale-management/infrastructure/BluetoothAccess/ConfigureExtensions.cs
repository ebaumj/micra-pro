using System.Runtime.InteropServices;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.ScaleManagement.Infrastructure.BluetoothAccess;

internal static class ConfigureExtensions
{
    public static IServiceCollection AddBluetoothService(this IServiceCollection services)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            services
                .AddSingleton<LinuxBluetooth.BluetoothService>()
                .AddHostedService(p => p.GetRequiredService<LinuxBluetooth.BluetoothService>())
                .AddSingleton<IBluetoothService>(provider =>
                    provider.GetRequiredService<LinuxBluetooth.BluetoothService>()
                );
        }
        else
            services.AddTransient<IBluetoothService, InTheHand.BluetoothService>();
        return services;
    }
}
