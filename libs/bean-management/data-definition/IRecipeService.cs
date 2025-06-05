using MicraPro.BeanManagement.DataDefinition.ValueObjects;

namespace MicraPro.BeanManagement.DataDefinition;

public interface IRecipeService
{
    Task<IRecipe> AddRecipeAsync(RecipeProperties properties, Guid beanId, CancellationToken ct);
    public Task<IEnumerable<IRecipe>> GetRecipesAsync(CancellationToken ct);
    public Task<IRecipe> GetRecipeAsync(Guid recipeId, CancellationToken ct);
    Task<IRecipe> UpdateRecipeAsync(
        Guid recipeId,
        RecipeProperties properties,
        CancellationToken ct
    );
    Task<Guid> RemoveRecipeAsync(Guid recipeId, CancellationToken ct);
}
