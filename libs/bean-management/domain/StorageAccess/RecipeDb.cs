using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.BeanManagement.Domain.StorageAccess;

public abstract class RecipeDb(Guid id, Guid beanId) : IEntity
{
    public Guid Id { get; } = id;
    public Guid BeanId { get; } = beanId;

    public BeanDb BeanObject { get; init; } = null!;
}

public class EspressoRecipeDb : RecipeDb
{
    public double GrindSetting { get; set; }
    public double CoffeeQuantity { get; set; }
    public double InCupQuantity { get; set; }
    public double BrewTemperature { get; set; }
    public TimeSpan TargetExtractionTime { get; set; }

    private EspressoRecipeDb(
        Guid id,
        Guid beanId,
        double grindSetting,
        double coffeeQuantity,
        double inCupQuantity,
        double brewTemperature,
        TimeSpan targetExtractionTime
    )
        : base(id, beanId)
    {
        GrindSetting = grindSetting;
        CoffeeQuantity = coffeeQuantity;
        InCupQuantity = inCupQuantity;
        BrewTemperature = brewTemperature;
        TargetExtractionTime = targetExtractionTime;
    }

    public EspressoRecipeDb(
        Guid beanId,
        double grindSetting,
        double coffeeQuantity,
        double inCupQuantity,
        double brewTemperature,
        TimeSpan targetExtractionTime
    )
        : this(
            Guid.NewGuid(),
            beanId,
            grindSetting,
            coffeeQuantity,
            inCupQuantity,
            brewTemperature,
            targetExtractionTime
        ) { }
}

public class V60RecipeDb : RecipeDb
{
    public double GrindSetting { get; set; }
    public double CoffeeQuantity { get; set; }
    public double InCupQuantity { get; set; }
    public double BrewTemperature { get; set; }

    private V60RecipeDb(
        Guid id,
        Guid beanId,
        double grindSetting,
        double coffeeQuantity,
        double inCupQuantity,
        double brewTemperature
    )
        : base(id, beanId)
    {
        GrindSetting = grindSetting;
        CoffeeQuantity = coffeeQuantity;
        InCupQuantity = inCupQuantity;
        BrewTemperature = brewTemperature;
    }

    public V60RecipeDb(
        Guid beanId,
        double grindSetting,
        double coffeeQuantity,
        double inCupQuantity,
        double brewTemperature
    )
        : this(Guid.NewGuid(), beanId, grindSetting, coffeeQuantity, inCupQuantity, brewTemperature)
    { }
}
