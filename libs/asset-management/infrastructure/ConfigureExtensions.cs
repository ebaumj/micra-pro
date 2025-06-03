using MicraPro.AssetManagement.Domain.AssetAccess;
using MicraPro.AssetManagement.Domain.StorageAccess;
using MicraPro.AssetManagement.Infrastructure.AssetAccess;
using MicraPro.AssetManagement.Infrastructure.Interfaces;
using MicraPro.AssetManagement.Infrastructure.Services;
using MicraPro.AssetManagement.Infrastructure.StorageAccess;
using MicraPro.Shared.UtilsDotnet;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.AssetManagement.Infrastructure;

public static class ConfigureExtensions
{
    public static IServiceCollection AddAssetManagementInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configurationManager
    )
    {
        DotNetEnv.Env.Load();
        var runtimeOptions = new AssetManagementInfrastructureRuntimeOptions()
        {
            RemoteAssetServerPrivateKey = Environment
                .GetEnvironmentVariable("REMOTE_ASSET_SERVER_PRIVATE_KEY")!
                .Select(c => (byte)c)
                .ToArray(),
        };
        return services
            .Configure<AssetManagementInfrastructureOptions>(
                configurationManager.GetSection(AssetManagementInfrastructureOptions.SectionName)
            )
            .Configure<AssetManagementInfrastructureRuntimeOptions>(options =>
            {
                options.RemoteAssetServerPrivateKey = runtimeOptions.RemoteAssetServerPrivateKey;
            })
            .AddSingleton<IAssetDirectoryService, AssetDirectoryService>()
            .AddSingleton<IRemoteAssetService, RemoteAssetService>()
            .AddTransient<ITokenCreatorService, TokenCreatorService>()
            .AddTransient<IHttpClientWrapperFactory, HttpClientWrapperFactory>()
            .AddTransient<IFileSystemAccess, FileSystemAccess>()
            .AddScoped<IAssetRepository, AssetRepository>()
            .AddDbContextAndMigrationService<AssetManagementDbContext>();
    }
}
