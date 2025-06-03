using MicraPro.BeanManagement.DataDefinition;
using MicraPro.BeanManagement.DataDefinition.ValueObjects;

namespace MicraPro.BeanManagement.Domain.ValueObjects;

public record Recipe(Guid Id, Guid BeanId, RecipeProperties Properties) : IRecipe;
