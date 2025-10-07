namespace MicraPro.Shared.Infrastructure;

public class SharedInfrastructureOptions
{
    public static string SectionName { get; } =
        typeof(SharedInfrastructureOptions).Namespace!.Replace('.', ':');
    public string WifiAdapter { get; set; } = string.Empty;
}
