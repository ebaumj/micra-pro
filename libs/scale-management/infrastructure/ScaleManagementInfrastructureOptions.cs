namespace MicraPro.ScaleManagement.Infrastructure;

public class ScaleManagementInfrastructureOptions
{
    public static string SectionName { get; } =
        typeof(ScaleManagementInfrastructureOptions).Namespace!.Replace('.', ':');
    public string LinuxBluetoothAdapterName { get; set; } = string.Empty;
}
