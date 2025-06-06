namespace MicraPro.AssetManagement.Domain.AssetAccess;

// Transient
public interface IAssetDirectoryService
{
    string CreateRandomFileNameWithoutExtension();
    Task<IEnumerable<string>> GetFilesAsync(CancellationToken ct);
    Task WriteFileAsync(string path, byte[] content, CancellationToken ct);
    Task RemoveFileAsync(string path, CancellationToken ct);
}
