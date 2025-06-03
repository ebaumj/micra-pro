namespace MicraPro.BrewByWeight.Infrastructure;

public class BrewByWeightInfrastructureOptions
{
    public static string SectionName { get; } =
        typeof(BrewByWeightInfrastructureOptions).Namespace!.Replace('.', ':');
    public int RelaisGpio { get; set; } = 0;
}
