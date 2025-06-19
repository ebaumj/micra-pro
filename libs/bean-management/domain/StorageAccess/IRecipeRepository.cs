using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.BeanManagement.Domain.StorageAccess;

public interface IRecipeRepository : IRepository<RecipeDb>
{
    Task<EspressoRecipeDb> UpdateEspressoAsync(
        Guid recipeId,
        double grindSetting,
        double coffeeQuantity,
        double inCupQuantity,
        double brewTemperature,
        TimeSpan targetExtractionTime,
        CancellationToken ct
    );

    Task<V60RecipeDb> UpdateV60Async(
        Guid recipeId,
        double grindSetting,
        double coffeeQuantity,
        double inCupQuantity,
        double brewTemperature,
        CancellationToken ct
    );
}
