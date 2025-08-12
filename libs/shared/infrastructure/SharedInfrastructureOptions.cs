namespace MicraPro.Shared.Infrastructure;

public class SharedInfrastructureOptions
{
    public static string SectionName { get; } =
        typeof(SharedInfrastructureOptions).Namespace!.Replace('.', ':');
    public string WifiAdapter { get; set; } = string.Empty;
    public string BluetoothAdapter { get; set; } = string.Empty;
    public int BrewPaddleRelaisGpio { get; set; } = 0;
    public string SystemVersion { get; set; } = string.Empty;
    public string UpdateDestination { get; set; } = string.Empty;
    public string UpdateFileName { get; set; } = string.Empty;
    public string UpdatePublicKey { get; set; } = string.Empty;
    public bool AllowUpdates { get; set; } = false;
    public string BrewPaddleAccess { get; set; } = string.Empty;
}
