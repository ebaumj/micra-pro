using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.Shared.UtilsDotnet;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDbContextAndMigrationService<TDbContext>(
        this IServiceCollection services
    )
        where TDbContext : DbContext =>
        services
            .AddDbContext<TDbContext>()
            .AddSingleton<MigrationService<TDbContext>>()
            .AddHostedService(p => p.GetRequiredService<MigrationService<TDbContext>>())
            .AddScoped<MigratedContextProvider<TDbContext>>();
}
