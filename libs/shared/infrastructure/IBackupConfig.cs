namespace MicraPro.Shared.Infrastructure;

public interface IBackupConfig
{
    public record BackupConfig(string Server, string Directory, string User, string Password);

    public BackupConfig? Config { get; }
}
