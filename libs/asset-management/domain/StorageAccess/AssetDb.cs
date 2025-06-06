using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.AssetManagement.Domain.StorageAccess;

public class AssetDb : IEntity
{
    public Guid Id { get; }
    public string RelativePath { get; set; }

    private AssetDb(Guid id, string relativePath)
    {
        Id = id;
        RelativePath = relativePath;
    }

    public AssetDb(string relativePath)
        : this(Guid.NewGuid(), relativePath) { }
}
