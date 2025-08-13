namespace MicraPro.AssetManagement.Domain.AssetAccess;

public interface IAssetDirectoryService
{
    IEnumerable<string> Files { get; }
    string LocalServerPath(string fileName);
    string CreateRandomFileNameWithoutExtension();
    Task ReadFilesAsync(CancellationToken ct);
    Task WriteFileAsync(string path, byte[] content, CancellationToken ct);
    Task RemoveFileAsync(string path, CancellationToken ct);
}
