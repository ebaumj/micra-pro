using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.BeanManagement.Domain.StorageAccess;

public interface IRecipeRepository : IRepository<RecipeDb>
{
    Task<RecipeDb> UpdateEspressoAsync(
        Guid recipeId,
        double grindSetting,
        double coffeeQuantity,
        double inCupQuantity,
        double brewTemperature,
        TimeSpan targetExtractionTime,
        CancellationToken ct
    );

    Task<RecipeDb> UpdateV60Async(
        Guid recipeId,
        double grindSetting,
        double coffeeQuantity,
        double inCupQuantity,
        double brewTemperature,
        CancellationToken ct
    );
}
