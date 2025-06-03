namespace MicraPro.AssetManagement.Infrastructure.Interfaces;

public interface IFileSystemAccess
{
    bool DirectoryExists(string path);
    void CreateDirectory(string path);
    string[] GetFiles(string path, string searchPattern, SearchOption searchOption);
    Task WriteFileAsync(string path, byte[] content, CancellationToken ct);
    void DeleteFile(string path);
}
