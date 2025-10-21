using MicraPro.BeanManagement.DataDefinition;
using MicraPro.BeanManagement.DataDefinition.ValueObjects;
using MicraPro.BeanManagement.Domain.StorageAccess;

namespace MicraPro.BeanManagement.Domain.Services;

public class RecipeService(IRecipeRepository recipeRepository, IGrinderSettings grinderSettings)
    : IRecipeService
{
    public async Task<IRecipe> AddRecipeAsync(
        RecipeProperties properties,
        Guid beanId,
        CancellationToken ct
    )
    {
        var entity = properties
            .WithGrinderOffset(0 - await grinderSettings.GetGrinderOffset(ct))
            .ToRecipeDb(beanId);
        await recipeRepository.AddAsync(entity, ct);
        await recipeRepository.SaveAsync(ct);
        return entity.ToRecipe();
    }

    public async Task<IEnumerable<IRecipe>> GetRecipesAsync(CancellationToken ct) =>
        await Task.WhenAll(
            (await recipeRepository.GetAllAsync(ct))
                .Select(entity => entity.ToRecipe())
                .Select(async r =>
                    r with
                    {
                        Properties = r.Properties.WithGrinderOffset(
                            await grinderSettings.GetGrinderOffset(ct)
                        ),
                    }
                )
        );

    public async Task<IRecipe> UpdateRecipeAsync(
        Guid recipeId,
        RecipeProperties properties,
        CancellationToken ct
    ) =>
        await recipeRepository.Update(
            recipeId,
            properties.WithGrinderOffset(0 - await grinderSettings.GetGrinderOffset(ct)),
            ct
        );

    public async Task<Guid> RemoveRecipeAsync(Guid recipeId, CancellationToken ct)
    {
        await recipeRepository.DeleteAsync(recipeId, ct);
        await recipeRepository.SaveAsync(ct);
        return recipeId;
    }
}
