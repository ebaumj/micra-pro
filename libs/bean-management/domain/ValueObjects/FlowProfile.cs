using MicraPro.BeanManagement.DataDefinition;
using MicraPro.BeanManagement.DataDefinition.ValueObjects;
using MicraPro.BeanManagement.Domain.StorageAccess;

namespace MicraPro.BeanManagement.Domain.ValueObjects;

public record FlowProfile(Guid Id, Guid RecipeId, FlowProfileProperties Properties) : IFlowProfile
{
    public FlowProfile(FlowProfileDb db)
        : this(db.Id, db.RecipeId, new FlowProfileProperties(db.StartFlow, db.FlowSettings)) { }
}
