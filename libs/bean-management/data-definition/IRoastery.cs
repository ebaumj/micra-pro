using MicraPro.BeanManagement.DataDefinition.ValueObjects;

namespace MicraPro.BeanManagement.DataDefinition;

public interface IRoastery
{
    Guid Id { get; }
    RoasteryProperties Properties { get; }
}
