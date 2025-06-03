using MicraPro.BeanManagement.DataDefinition;
using MicraPro.BeanManagement.DataDefinition.ValueObjects;
using MicraPro.BeanManagement.Domain.StorageAccess;

namespace MicraPro.BeanManagement.Domain.Services;

public class RecipeService(IRecipeRepository recipeRepository) : IRecipeService
{
    public async Task<IRecipe> AddRecipeAsync(
        RecipeProperties properties,
        Guid beanId,
        CancellationToken ct
    )
    {
        var entity = properties.ToRecipeDb(beanId);
        await recipeRepository.AddAsync(entity, ct);
        await recipeRepository.SaveAsync(ct);
        return entity.ToRecipe();
    }

    public async Task<IEnumerable<IRecipe>> GetRecipesAsync(CancellationToken ct) =>
        (await recipeRepository.GetAllAsync(ct)).Select(entity => entity.ToRecipe());

    public async Task<IRecipe> GetRecipeAsync(Guid recipeId, CancellationToken ct) =>
        (await recipeRepository.GetByIdAsync(recipeId, ct)).ToRecipe();

    public async Task<IRecipe> UpdateRecipeAsync(
        Guid recipeId,
        RecipeProperties properties,
        CancellationToken ct
    ) => await recipeRepository.Update(recipeId, properties, ct);

    public async Task<Guid> RemoveRecipeAsync(Guid recipeId, CancellationToken ct)
    {
        await recipeRepository.DeleteAsync(recipeId, ct);
        await recipeRepository.SaveAsync(ct);
        return recipeId;
    }
}
