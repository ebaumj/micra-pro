using MicraPro.AssetManagement.Domain.AssetAccess;
using Microsoft.Extensions.Options;

namespace MicraPro.AssetManagement.Infrastructure.AssetAccess;

public class AssetDirectoryService(IOptions<AssetManagementInfrastructureOptions> options)
    : IAssetDirectoryService
{
    public string CreateRandomFileNameWithoutExtension() => Path.GetRandomFileName();

    public Task<IEnumerable<string>> GetFilesAsync(CancellationToken ct)
    {
        if (!Directory.Exists(options.Value.LocalFileServerFolder))
            Directory.CreateDirectory(options.Value.LocalFileServerFolder);
        return Task.FromResult<IEnumerable<string>>(
            Directory.GetFiles(
                options.Value.LocalFileServerFolder,
                "*.*",
                SearchOption.AllDirectories
            )
        );
    }

    public Task WriteFileAsync(string path, byte[] content, CancellationToken ct)
    {
        if (!Directory.Exists(options.Value.LocalFileServerFolder))
            Directory.CreateDirectory(options.Value.LocalFileServerFolder);
        return File.WriteAllBytesAsync(
            Path.Combine(options.Value.LocalFileServerFolder, path),
            content,
            ct
        );
    }

    public Task RemoveFileAsync(string path, CancellationToken ct)
    {
        try
        {
            File.Delete(Path.Combine(options.Value.LocalFileServerFolder, path));
        }
        catch (FileNotFoundException)
        {
            // File is already removed
        }
        return Task.CompletedTask;
    }
}
