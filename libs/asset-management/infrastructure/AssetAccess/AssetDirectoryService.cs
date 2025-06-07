using MicraPro.AssetManagement.Domain.AssetAccess;
using Microsoft.Extensions.Options;

namespace MicraPro.AssetManagement.Infrastructure.AssetAccess;

public class AssetDirectoryService(IOptions<AssetManagementInfrastructureOptions> options)
    : IAssetDirectoryService
{
    public string CreateRandomFileNameWithoutExtension() => Path.GetRandomFileName();

    private List<string> _files = [];

    public IEnumerable<string> Files => _files;

    public string LocalServerPath(string fileName) =>
        $"{options.Value.LocalFileServerDomain}/{fileName}";

    public async Task WriteFileAsync(string path, byte[] content, CancellationToken ct)
    {
        if (!Directory.Exists(options.Value.LocalFileServerFolder))
            Directory.CreateDirectory(options.Value.LocalFileServerFolder);
        await File.WriteAllBytesAsync(
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
            File.Delete(Path.Join(options.Value.LocalFileServerFolder, path));
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
        if (!Directory.Exists(options.Value.LocalFileServerFolder))
            Directory.CreateDirectory(options.Value.LocalFileServerFolder);
        _files = Directory
            .GetFiles(options.Value.LocalFileServerFolder, "*.*", SearchOption.AllDirectories)
            .Select(f => Path.GetRelativePath(options.Value.LocalFileServerFolder, f))
            .ToList();
        return Task.CompletedTask;
    }
}
