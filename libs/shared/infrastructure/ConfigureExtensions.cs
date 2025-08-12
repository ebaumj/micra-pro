using System.Runtime.InteropServices;
using MicraPro.Shared.Domain;
using MicraPro.Shared.Domain.BluetoothAccess;
using MicraPro.Shared.Domain.KeyValueStore;
using MicraPro.Shared.Domain.WiredConnections;
using MicraPro.Shared.Infrastructure.BluetoothAccess;
using MicraPro.Shared.Infrastructure.KeyValueStore;
using MicraPro.Shared.Infrastructure.WiredConnections;
using MicraPro.Shared.UtilsDotnet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.Shared.Infrastructure;

public static class ConfigureExtensions
{
    public static IServiceCollection AddSharedInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configurationManager
    )
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            services
                .AddSingleton<SystemService>()
                .AddSingleton<ISystemService>(sp => sp.GetRequiredService<SystemService>())
                .AddSingleton<IWifiEnableService>(sp => sp.GetRequiredService<SystemService>())
                .AddHostedService<WifiEnableHostService>()
                .AddBrewPaddle(configurationManager)
                .AddSingleton<BluetoothService>()
                .AddHostedService(p => p.GetRequiredService<BluetoothService>())
                .AddSingleton<IBluetoothService>(provider =>
                    provider.GetRequiredService<BluetoothService>()
                );
        else
            services
                .AddSingleton<ISystemService, SystemServiceDummy>()
                .AddTransient<IBrewPaddleAccess, BrewPaddleAccessDummy>()
                .AddTransient<IBluetoothService, DummyBluetoothService>();
        return services
            .Configure<SharedInfrastructureOptions>(
                configurationManager.GetSection(SharedInfrastructureOptions.SectionName)
            )
            .AddDbContextAndMigrationService<SharedDbContext>()
            .AddScoped<IConfigurationRepository, ConfigurationRepository>()
            .AddScoped<IKeyValueStoreProvider, KeyValueStoreProvider>()
            .AddScoped<KeyValueStoreBase>();
    }

    private static IServiceCollection AddBrewPaddle(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        if (
            configuration
                .GetSection(SharedInfrastructureOptions.SectionName)
                .GetValue<string>("BrewPaddleAccess") == "Serial"
        )
            services.AddBrewPaddleSerial();
        else
            services.AddBrewPaddleGpio();
        return services;
    }

    private static IServiceCollection AddBrewPaddleGpio(this IServiceCollection services) =>
        services
            .AddSingleton<BrewPaddleAccess>()
            .AddSingleton<IBrewPaddleAccess>(sp => sp.GetRequiredService<BrewPaddleAccess>())
            .AddHostedService(sp => sp.GetRequiredService<BrewPaddleAccess>());

    private static IServiceCollection AddBrewPaddleSerial(this IServiceCollection services) =>
        services
            .AddSingleton<BrewPaddleAccessSerial>()
            .AddSingleton<IBrewPaddleAccess>(sp => sp.GetRequiredService<BrewPaddleAccessSerial>())
            .AddHostedService(sp => sp.GetRequiredService<BrewPaddleAccessSerial>());
}
