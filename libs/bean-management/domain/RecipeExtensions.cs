using MicraPro.BeanManagement.DataDefinition.ValueObjects;
using MicraPro.BeanManagement.Domain.StorageAccess;
using MicraPro.BeanManagement.Domain.ValueObjects;

namespace MicraPro.BeanManagement.Domain;

public static class RecipeExtensions
{
    public static RecipeDb ToRecipeDb(this RecipeProperties properties, Guid beanId) =>
        properties switch
        {
            RecipeProperties.Espresso p => new EspressoRecipeDb(
                beanId,
                p.GrindSetting,
                p.CoffeeQuantity,
                p.InCupQuantity,
                p.BrewTemperature,
                p.TargetExtractionTime
            ),
            RecipeProperties.V60 p => new V60RecipeDb(
                beanId,
                p.GrindSetting,
                p.CoffeeQuantity,
                p.InCupQuantity,
                p.BrewTemperature
            ),
            _ => throw new NotImplementedException(
                $"Recipe Properties Type {properties.GetType().Name} is not implemented!"
            ),
        };

    public static Recipe ToRecipe(this RecipeDb db) =>
        new(
            db.Id,
            db.BeanId,
            db switch
            {
                EspressoRecipeDb r => new RecipeProperties.Espresso(
                    r.GrindSetting,
                    r.CoffeeQuantity,
                    r.InCupQuantity,
                    r.BrewTemperature,
                    r.TargetExtractionTime
                ),
                V60RecipeDb r => new RecipeProperties.V60(
                    r.GrindSetting,
                    r.CoffeeQuantity,
                    r.InCupQuantity,
                    r.BrewTemperature
                ),
                _ => throw new NotImplementedException(
                    $"Recipe Type {db.GetType().Name} is not implemented!"
                ),
            }
        );

    public static async Task<Recipe> Update(
        this IRecipeRepository repository,
        Guid recipeId,
        RecipeProperties properties,
        CancellationToken ct
    ) =>
        properties switch
        {
            RecipeProperties.Espresso r => (
                await repository.UpdateEspressoAsync(
                    recipeId,
                    r.GrindSetting,
                    r.CoffeeQuantity,
                    r.InCupQuantity,
                    r.BrewTemperature,
                    r.TargetExtractionTime,
                    ct
                )
            ).ToRecipe(),
            RecipeProperties.V60 r => (
                await repository.UpdateV60Async(
                    recipeId,
                    r.GrindSetting,
                    r.CoffeeQuantity,
                    r.InCupQuantity,
                    r.BrewTemperature,
                    ct
                )
            ).ToRecipe(),
            _ => throw new NotImplementedException(
                $"Recipe Properties Type {properties.GetType().Name} is not implemented!"
            ),
        };

    public static RecipeProperties WithGrinderOffset(
        this RecipeProperties properties,
        double offset
    ) =>
        properties switch
        {
            RecipeProperties.Espresso espresso => espresso with
            {
                GrindSetting = espresso.GrindSetting + offset,
            },
            RecipeProperties.V60 v60 => v60 with { GrindSetting = v60.GrindSetting + offset },
            _ => throw new NotImplementedException(
                $"Recipe Properties Type {properties.GetType().Name} is not implemented!"
            ),
        };
}
