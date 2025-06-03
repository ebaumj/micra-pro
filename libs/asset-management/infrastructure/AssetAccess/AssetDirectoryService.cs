using MicraPro.AssetManagement.Domain.AssetAccess;
using MicraPro.AssetManagement.Infrastructure.Interfaces;
using Microsoft.Extensions.Options;

namespace MicraPro.AssetManagement.Infrastructure.AssetAccess;

public class AssetDirectoryService(
    IFileSystemAccess fileSystemAccess,
    IOptions<AssetManagementInfrastructureOptions> options
) : IAssetDirectoryService
{
    public string CreateRandomFileNameWithoutExtension() => Path.GetRandomFileName();

    private List<string> _files = [];

    public IEnumerable<string> Files => _files;

    public string LocalServerPath(string fileName) =>
        $"{options.Value.LocalFileServerDomain}/{fileName}";

    public async Task WriteFileAsync(string path, byte[] content, CancellationToken ct)
    {
        if (!fileSystemAccess.DirectoryExists(options.Value.LocalFileServerFolder))
            fileSystemAccess.CreateDirectory(options.Value.LocalFileServerFolder);
        await fileSystemAccess.WriteFileAsync(
            Path.Join(options.Value.LocalFileServerFolder, path),
            content,
            ct
        );
        _files = _files.Concat([path]).ToList();
    }

    public Task RemoveFileAsync(string path, CancellationToken ct)
    {
        try
        {
            fileSystemAccess.DeleteFile(Path.Join(options.Value.LocalFileServerFolder, path));
        }
        catch (FileNotFoundException)
        {
            // File is already removed
        }
        _files = _files.Where(f => f != path).ToList();
        return Task.CompletedTask;
    }

    public Task ReadFilesAsync(CancellationToken ct)
    {
        if (!fileSystemAccess.DirectoryExists(options.Value.LocalFileServerFolder))
            fileSystemAccess.CreateDirectory(options.Value.LocalFileServerFolder);
        _files = fileSystemAccess
            .GetFiles(options.Value.LocalFileServerFolder, "*.*", SearchOption.AllDirectories)
            .Select(f => Path.GetRelativePath(options.Value.LocalFileServerFolder, f))
            .ToList();
        return Task.CompletedTask;
    }
}
