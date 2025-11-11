using System.Runtime.InteropServices;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.ScaleManagement.Infrastructure.BluetoothAccess;

internal static class ConfigureExtensions
{
    public static IServiceCollection AddBluetoothService(this IServiceCollection services)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            services.AddTransient<IBluetoothService, LinuxBluetooth.BluetoothService>();
        else
            services.AddTransient<IBluetoothService, Dummy.DummyBluetoothService>();
        return services;
    }
}
