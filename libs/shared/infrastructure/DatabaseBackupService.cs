using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Renci.SshNet;

namespace MicraPro.Shared.Infrastructure;

public class DatabaseBackupService(
    IConfiguration configuration,
    ILogger<DatabaseBackupService> logger
)
{
    private const string FileName = "database.db";

    public async Task<string?> BackupDatabaseAsync(
        string server,
        string directory,
        string username,
        string password,
        CancellationToken ct
    )
    {
        logger.LogInformation("Start Database Backup");
        var backupFilePath = Path.Combine(Path.GetTempPath(), FileName);
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
        var remoteDirectory = $"{directory}/{DateTime.UtcNow:yyyyMMdd_HHmmss}";
        var remoteFilePath = $"{remoteDirectory}/{FileName}";
        try
        {
            using var sftpClient = new SftpClient(server, username, password);
            await sftpClient.ConnectAsync(ct);
            if (sftpClient.IsConnected)
            {
                await using var localFileStream = File.OpenRead(backupFilePath);
                await sftpClient.CreateDirectoryAsync(remoteDirectory, ct);
                await sftpClient.UploadFileAsync(localFileStream, remoteFilePath, ct);
                sftpClient.Disconnect();
                logger.LogInformation("Database backup complete");
                return remoteDirectory;
            }
            logger.LogError("Failed to connect to SFTP Server {s}", server);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while uploading via SFTP to {s}. Details: {e}",
                server,
                ex.Message
            );
        }
        return null;
    }
}
