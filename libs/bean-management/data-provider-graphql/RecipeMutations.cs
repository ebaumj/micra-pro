using MicraPro.Auth.DataDefinition;
using MicraPro.BeanManagement.DataDefinition;
using MicraPro.BeanManagement.DataDefinition.ValueObjects;
using MicraPro.BeanManagement.DataProviderGraphQl.Types;

namespace MicraPro.BeanManagement.DataProviderGraphQl;

[MutationType]
public static class RecipeMutations
{
    [RequiredPermissions([Permission.WriteRecipes])]
    public static async Task<RecipeApi> AddEspressoRecipe(
        [Service] IRecipeService recipeService,
        Guid beanId,
        RecipeProperties.Espresso properties,
        CancellationToken ct
    ) => (await recipeService.AddRecipeAsync(properties, beanId, ct)).ToApi();

    [RequiredPermissions([Permission.WriteRecipes])]
    public static async Task<RecipeApi> AddV60Recipe(
        [Service] IRecipeService recipeService,
        Guid beanId,
        RecipeProperties.V60 properties,
        CancellationToken ct
    ) => (await recipeService.AddRecipeAsync(properties, beanId, ct)).ToApi();

    [RequiredPermissions([Permission.WriteRecipes])]
    public static async Task<Guid> RemoveRecipe(
        [Service] IRecipeService recipeService,
        Guid recipeId,
        CancellationToken ct
    )
    {
        await recipeService.RemoveRecipeAsync(recipeId, ct);
        return recipeId;
    }

    [RequiredPermissions([Permission.WriteRecipes])]
    public static async Task<RecipeApi> UpdateEspressoRecipe(
        [Service] IRecipeService recipeService,
        Guid recipeId,
        RecipeProperties.Espresso properties,
        CancellationToken ct
    ) => (await recipeService.UpdateRecipeAsync(recipeId, properties, ct)).ToApi();

    [RequiredPermissions([Permission.WriteRecipes])]
    public static async Task<RecipeApi> UpdateV60Recipe(
        [Service] IRecipeService recipeService,
        Guid recipeId,
        RecipeProperties.V60 properties,
        CancellationToken ct
    ) => (await recipeService.UpdateRecipeAsync(recipeId, properties, ct)).ToApi();
}
