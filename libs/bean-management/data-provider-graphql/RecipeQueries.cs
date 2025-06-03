using MicraPro.Auth.DataDefinition;
using MicraPro.BeanManagement.DataDefinition;
using MicraPro.BeanManagement.DataProviderGraphQl.Types;

namespace MicraPro.BeanManagement.DataProviderGraphQl;

[QueryType]
public static class RecipeQueries
{
    [RequiredPermissions([Permission.ReadRecipes])]
    public static async Task<List<RecipeApi>> GetRecipes(
        [Service] IRecipeService roasteryService,
        CancellationToken ct
    ) => (await roasteryService.GetRecipesAsync(ct)).Select(r => r.ToApi()).ToList();
}
