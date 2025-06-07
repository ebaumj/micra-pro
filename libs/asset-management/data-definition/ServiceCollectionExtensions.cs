using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.AssetManagement.DataDefinition;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddScopedAssetConsumer<TInterface, TImplementation>(
        this IServiceCollection services
    )
        where TImplementation : class, TInterface, IAssetConsumer
        where TInterface : class =>
        services
            .AddScopedAssetConsumer<TImplementation>()
            .AddScoped<TInterface>(sp => sp.GetRequiredService<TImplementation>());

    private static IServiceCollection AddScopedAssetConsumer<TImplementation>(
        this IServiceCollection services
    )
        where TImplementation : class, IAssetConsumer =>
        services
            .AddScoped<TImplementation>()
            .AddScoped<IAssetConsumer>(sp => sp.GetRequiredService<TImplementation>());
}
