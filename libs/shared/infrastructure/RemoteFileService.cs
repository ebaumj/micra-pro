using Renci.SshNet;

namespace MicraPro.Shared.Infrastructure;

public class RemoteFileService(IBackupConfig config) : IRemoteFileService
{
    private readonly SftpClient? _client =
        config.Config == null
            ? null
            : new SftpClient(config.Config.Server, 22, config.Config.User, config.Config.Password);

    private SftpClient Client => _client ?? throw new NullReferenceException();

    public async Task ReadFileAsync(string localPath, string remotePath, CancellationToken ct)
    {
        await Client.ConnectAsync(ct);
        await using var localFileStream = File.OpenWrite(localPath);
        await Client.DownloadFileAsync(remotePath, localFileStream, ct);
        Client.Disconnect();
    }

    public async Task WriteFileAsync(string localPath, string remotePath, CancellationToken ct)
    {
        await Client.ConnectAsync(ct);
        await using var localFileStream = File.OpenRead(localPath);
        await Client.UploadFileAsync(localFileStream, remotePath, ct);
        Client.Disconnect();
    }

    public async Task CreateDirectoryAsync(string directory, CancellationToken ct)
    {
        await Client.ConnectAsync(ct);
        await Client.CreateDirectoryAsync(directory, ct);
        Client.Disconnect();
    }

    public async Task<IRemoteFileService.Item[]> ListDirectoryAsync(
        string directory,
        CancellationToken ct
    )
    {
        await Client.ConnectAsync(ct);
        var result = Client
            .ListDirectory(directory)
            .Select(f =>
                f.IsDirectory ? new IRemoteFileService.Item(f.Name, true)
                : f.IsRegularFile ? new IRemoteFileService.Item(f.Name, false)
                : null
            )
            .Where(i => i != null)
            .Select(i => i!)
            .ToArray();
        Client.Disconnect();
        return result;
    }

    public async Task DeleteDirectoryAsync(string directory, CancellationToken ct)
    {
        await Client.ConnectAsync(ct);
        await EmptyAndDeleteDirectoryRecursivelyAsync(directory, ct);
        Client.Disconnect();
    }

    private async Task EmptyAndDeleteDirectoryRecursivelyAsync(string path, CancellationToken ct)
    {
        var files = await Task.Run(() => Client.ListDirectory(path), ct);
        foreach (var file in files)
        {
            if (file.Name is "." or "..")
                continue;
            if (file.IsDirectory)
                await EmptyAndDeleteDirectoryRecursivelyAsync(file.FullName, ct);
            else
                await Client.DeleteFileAsync(file.FullName, ct);
        }
        await Client.DeleteDirectoryAsync(path, ct);
    }
}
