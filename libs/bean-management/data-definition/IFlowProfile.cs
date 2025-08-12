using MicraPro.BeanManagement.DataDefinition.ValueObjects;

namespace MicraPro.BeanManagement.DataDefinition;

public interface IFlowProfile
{
    Guid Id { get; }
    Guid RecipeId { get; }
    FlowProfileProperties Properties { get; }
}
