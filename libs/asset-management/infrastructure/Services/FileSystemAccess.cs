using MicraPro.AssetManagement.Infrastructure.Interfaces;

namespace MicraPro.AssetManagement.Infrastructure.Services;

public class FileSystemAccess : IFileSystemAccess
{
    public bool DirectoryExists(string path) => Directory.Exists(path);

    public void CreateDirectory(string path) => Directory.CreateDirectory(path);

    public string[] GetFiles(string path, string searchPattern, SearchOption searchOption) =>
        Directory.GetFiles(path, searchPattern, searchOption);

    public Task WriteFileAsync(string path, byte[] content, CancellationToken ct) =>
        File.WriteAllBytesAsync(path, content, ct);

    public void DeleteFile(string path) => File.Delete(path);
}
