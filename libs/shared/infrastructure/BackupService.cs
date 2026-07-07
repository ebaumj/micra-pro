using System.Data.Common;
using System.Globalization;
using System.Reactive.Linq;
using MicraPro.AssetManagement.DataDefinition;
using MicraPro.Shared.Domain;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MicraPro.Shared.Infrastructure;

public class BackupService(
    IOptions<SharedInfrastructureOptions> options,
    IConfiguration configuration,
    IBackupConfig config,
    IRemoteFileService remoteFileService,
    ILogger<BackupService> logger,
    IAssetService assetService,
    IHostApplicationLifetime appLifetime
) : IBackupService
{
    private const string DatabaseFileName = "database.db";

    public bool UseBackups => options.Value.UseBackups;

    public static void RestoreDatabaseFile(IConfiguration configuration)
    {
        var restoreFile = configuration.GetConnectionString("RestoreFileConnection");
        var defaultConnectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(defaultConnectionString) || string.IsNullOrEmpty(restoreFile))
            return;
        var builder = new DbConnectionStringBuilder { ConnectionString = defaultConnectionString };
        if (
            !builder.TryGetValue("Data Source", out var dataSourceObj)
            || dataSourceObj is not string dbFilePath
        )
            return;
        if (!File.Exists(restoreFile))
            return;
        if (File.Exists(dbFilePath))
            File.Delete(dbFilePath);
        File.Copy(restoreFile, dbFilePath);
        File.Delete(restoreFile);
    }

    public async Task<IBackupService.Backup[]> GetBackupsAsync(CancellationToken ct)
    {
        if (!UseBackups || config.Config == null)
            return [];
        return (await remoteFileService.ListDirectoryAsync(config.Config.Directory, ct))
            .Where(i => i.IsDirectory)
            .Select(i =>
            {
                var timestamp = TimeStampFromFolder(i.Name);
                return timestamp.HasValue
                    ? new IBackupService.Backup(i.Name, timestamp.Value)
                    : null;
            })
            .Where(b => b != null)
            .Select(b => b!)
            .ToArray();
    }

    public async Task<bool> RestoreDataAsync(string directory, CancellationToken ct)
    {
        if (config.Config == null)
            return false;
        try
        {
            var restoreFilePath = configuration.GetConnectionString("RestoreFileConnection");
            if (restoreFilePath == null)
                return false;
            var remoteDirectory = $"{config.Config.Directory}/{directory}";
            await remoteFileService.ReadFileAsync(
                restoreFilePath,
                $"{remoteDirectory}/{DatabaseFileName}",
                ct
            );
            await assetService.RestoreAssetsAsync(
                config.Config.Server,
                remoteDirectory,
                config.Config.User,
                config.Config.Password,
                ct
            );
            ScheduleShutdown();
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError("An error occurred while restoring data. Details: {e}", ex.Message);
            return false;
        }
    }

    public async Task<bool> DeleteBackupAsync(string directory, CancellationToken ct)
    {
        if (config.Config == null)
            return false;
        try
        {
            var remoteDirectory = $"{config.Config.Directory}/{directory}";
            await remoteFileService.DeleteDirectoryAsync(remoteDirectory, ct);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while deleting backup. Details: {e}",
                ex.Message
            );
            return false;
        }
    }

    private void ScheduleShutdown() =>
        Observable
            .Timer(TimeSpan.FromMilliseconds(200))
            .Subscribe(_ => appLifetime.StopApplication());

    public async Task<bool> BackupDataAsync(CancellationToken ct)
    {
        if (!options.Value.UseBackups)
            return false;
        if (config.Config == null)
            return false;
        var remoteDirectory = await BackupDatabaseAsync(config.Config.Directory, ct);
        if (remoteDirectory == null)
            return false;
        await assetService.BackupAssetsAsync(
            config.Config.Server,
            remoteDirectory,
            config.Config.User,
            config.Config.Password,
            ct
        );
        return true;
    }

    private async Task<string?> BackupDatabaseAsync(string directory, CancellationToken ct)
    {
        var backupFilePath = Path.Combine(Path.GetTempPath(), DatabaseFileName);
        if (File.Exists(backupFilePath))
            File.Delete(backupFilePath);
        await using (
            var sourceDb = new SqliteConnection(
                configuration.GetConnectionString("DefaultConnection")
            )
        )
        await using (
            var destinationDb = new SqliteConnection($"Data Source={backupFilePath};Pooling=False;")
        )
        {
            sourceDb.Open();
            sourceDb.BackupDatabase(destinationDb);
            sourceDb.Close();
            destinationDb.Close();
        }
        var remoteDirectory = $"{directory}/{FolderName(DateTime.Now)}";
        var remoteFilePath = $"{remoteDirectory}/{DatabaseFileName}";
        try
        {
            await remoteFileService.CreateDirectoryAsync(remoteDirectory, ct);
            await remoteFileService.WriteFileAsync(backupFilePath, remoteFilePath, ct);
            return remoteDirectory;
        }
        catch (Exception ex)
        {
            logger.LogError("An error occurred while uploading via SFTP. Details: {e}", ex.Message);
            return null;
        }
        finally
        {
            if (File.Exists(backupFilePath))
                File.Delete(backupFilePath);
        }
    }

    private static string FolderName(DateTime timestamp) => $"{timestamp:yyyyMMdd_HHmmss}";

    private static DateTime? TimeStampFromFolder(string name) =>
        DateTime.TryParseExact(
            name,
            "yyyyMMdd_HHmmss",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out var parsedDate
        )
            ? parsedDate
            : null;
}
