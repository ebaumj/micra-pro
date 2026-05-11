namespace MicraPro.Shared.Domain;

public interface IBackupService
{
    public record Backup(string Directory, DateTime Timestamp);

    public bool UseBackups { get; }
    Task<Backup[]> GetBackupsAsync(CancellationToken ct);
    Task<bool> BackupDataAsync(CancellationToken ct);
    Task<bool> RestoreDataAsync(string directory, CancellationToken ct);
    Task<bool> DeleteBackupAsync(string directory, CancellationToken ct);
}
