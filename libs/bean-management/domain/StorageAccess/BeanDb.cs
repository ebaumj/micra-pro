using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.BeanManagement.Domain.StorageAccess;

public class BeanDb : IEntity
{
    public Guid Id { get; }
    public string Name { get; set; }
    public Guid RoasteryId { get; }
    public string CountryCode { get; set; }
    public Guid AssetId { get; set; }

    public RoasteryDb RoasteryObject { get; init; } = null!;

    public ICollection<RecipeDb> Recipes { get; } = new List<RecipeDb>();

    private BeanDb(Guid id, string name, Guid roasteryId, string countryCode, Guid assetId)
    {
        Id = id;
        Name = name;
        RoasteryId = roasteryId;
        CountryCode = countryCode;
        AssetId = assetId;
    }

    public BeanDb(string name, Guid roasteryId, string countryCode, Guid assetId)
        : this(Guid.NewGuid(), name, roasteryId, countryCode, assetId) { }
}
