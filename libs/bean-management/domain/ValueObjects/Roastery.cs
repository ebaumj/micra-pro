using MicraPro.BeanManagement.DataDefinition;
using MicraPro.BeanManagement.DataDefinition.ValueObjects;
using MicraPro.BeanManagement.Domain.StorageAccess;

namespace MicraPro.BeanManagement.Domain.ValueObjects;

public record Roastery(Guid Id, RoasteryProperties Properties) : IRoastery
{
    public Roastery(RoasteryDb db)
        : this(db.Id, new RoasteryProperties(db.Name, db.Location)) { }
}
