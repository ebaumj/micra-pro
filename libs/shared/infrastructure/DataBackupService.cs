using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using MicraPro.AssetManagement.DataDefinition;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MicraPro.Shared.Infrastructure;

public class DataBackupService(
    IOptions<SharedInfrastructureOptions> options,
    DatabaseBackupService databaseBackupService,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<DataBackupService> logger
) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!options.Value.UseBackups)
            return;
        if (!TryReadConfig(options.Value.BackupConfigFile, out BackupConfig? config))
            return;
        var remoteDirectory = await databaseBackupService.BackupDatabaseAsync(
            config.server,
            config.directory,
            config.username,
            config.password,
            cancellationToken
        );
        if (remoteDirectory == null)
            return;
        var assetService = serviceScopeFactory
            .CreateScope()
            .ServiceProvider.GetRequiredService<IAssetService>();
        await assetService.BackupAssetsAsync(
            config.server,
            remoteDirectory,
            config.username,
            config.password,
            cancellationToken
        );
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private bool TryReadConfig(string path, [NotNullWhen(true)] out BackupConfig? config)
    {
        config = null;
        try
        {
            if (!File.Exists(path))
                return false;
            config = JsonSerializer.Deserialize<BackupConfig>(File.ReadAllText(path));
            return config != null;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to read or parse the backup config at {Path}", path);
            return false;
        }
    }

    private record BackupConfig(
        // ReSharper disable once InconsistentNaming
        string server,
        // ReSharper disable once InconsistentNaming
        string directory,
        // ReSharper disable once InconsistentNaming
        string username,
        // ReSharper disable once InconsistentNaming
        string password
    );
}
