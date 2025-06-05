using MicraPro.BeanManagement.Domain.StorageAccess;
using MicraPro.Shared.UtilsDotnet;
using Microsoft.EntityFrameworkCore;

namespace MicraPro.BeanManagement.Infrastructure.StorageAccess;

public class RecipeRepository(MigratedContextProvider<BeanManagementDbContext> contextProvider)
    : BaseSqliteRepository<RecipeDb>,
        IRecipeRepository
{
    protected override async Task<DbSet<RecipeDb>> GetEntitiesAsync(CancellationToken ct) =>
        (await contextProvider.GetContextAsync(ct)).RecipeEntries;

    protected override async Task<DbContext> GetContextAsync(CancellationToken ct) =>
        await contextProvider.GetContextAsync(ct);

    public async Task<RecipeDb> UpdateEspressoAsync(
        Guid recipeId,
        double grindSetting,
        double coffeeQuantity,
        double inCupQuantity,
        double brewTemperature,
        TimeSpan targetExtractionTime,
        CancellationToken ct
    )
    {
        var entityBase = await GetByIdAsync(recipeId, ct);
        if (entityBase is not EspressoRecipeDb entity)
            throw new ArgumentException("Recipe Type can not be changed!");
        entity.GrindSetting = grindSetting;
        entity.CoffeeQuantity = coffeeQuantity;
        entity.InCupQuantity = inCupQuantity;
        entity.BrewTemperature = brewTemperature;
        entity.TargetExtractionTime = targetExtractionTime;
        await SaveAsync(ct);
        return entity;
    }

    public async Task<RecipeDb> UpdateV60Async(
        Guid recipeId,
        double grindSetting,
        double coffeeQuantity,
        double inCupQuantity,
        double brewTemperature,
        CancellationToken ct
    )
    {
        var entityBase = await GetByIdAsync(recipeId, ct);
        if (entityBase is not V60RecipeDb entity)
            throw new ArgumentException("Recipe Type can not be changed!");
        entity.GrindSetting = grindSetting;
        entity.CoffeeQuantity = coffeeQuantity;
        entity.InCupQuantity = inCupQuantity;
        entity.BrewTemperature = brewTemperature;
        await SaveAsync(ct);
        return entity;
    }
}
