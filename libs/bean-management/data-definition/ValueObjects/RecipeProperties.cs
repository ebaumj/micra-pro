namespace MicraPro.BeanManagement.DataDefinition.ValueObjects;

public abstract record RecipeProperties
{
    public record Espresso(
        double GrindSetting,
        double CoffeeQuantity,
        double InCupQuantity,
        double BrewTemperature,
        TimeSpan TargetExtractionTime
    ) : RecipeProperties;

    public record V60(
        double GrindSetting,
        double CoffeeQuantity,
        double InCupQuantity,
        double BrewTemperature
    ) : RecipeProperties;
}
