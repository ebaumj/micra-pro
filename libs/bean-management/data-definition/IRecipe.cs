using MicraPro.BeanManagement.DataDefinition.ValueObjects;

namespace MicraPro.BeanManagement.DataDefinition;

public interface IRecipe
{
    Guid Id { get; }
    Guid BeanId { get; }
    RecipeProperties Properties { get; }
}
