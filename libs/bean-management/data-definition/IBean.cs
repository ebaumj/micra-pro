using MicraPro.BeanManagement.DataDefinition.ValueObjects;

namespace MicraPro.BeanManagement.DataDefinition;

public interface IBean
{
    Guid Id { get; }
    Guid RoasteryId { get; }
    BeanProperties Properties { get; }
    IEnumerable<IRecipe> Recipes { get; }
}
