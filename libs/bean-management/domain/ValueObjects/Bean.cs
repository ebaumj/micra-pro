using MicraPro.BeanManagement.DataDefinition;
using MicraPro.BeanManagement.DataDefinition.ValueObjects;
using MicraPro.BeanManagement.Domain.StorageAccess;

namespace MicraPro.BeanManagement.Domain.ValueObjects;

public record Bean(
    Guid Id,
    Guid RoasteryId,
    BeanProperties Properties,
    IEnumerable<IRecipe> Recipes
) : IBean
{
    public Bean(BeanDb db)
        : this(
            db.Id,
            db.RoasteryId,
            new BeanProperties(
                db.Name,
                db.CountryCode,
                db.AssetId == Guid.Empty ? null : db.AssetId
            ),
            db.Recipes.Select(r => r.ToRecipe())
        ) { }
}
