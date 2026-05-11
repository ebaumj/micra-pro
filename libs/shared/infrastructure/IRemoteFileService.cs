namespace MicraPro.Shared.Infrastructure;

public interface IRemoteFileService
{
    public record Item(string Name, bool IsDirectory);

    Task ReadFileAsync(string localPath, string remotePath, CancellationToken ct);
    Task WriteFileAsync(string localPath, string remotePath, CancellationToken ct);
    Task CreateDirectoryAsync(string directory, CancellationToken ct);
    Task<Item[]> ListDirectoryAsync(string directory, CancellationToken ct);
    Task DeleteDirectoryAsync(string directory, CancellationToken ct);
}
