namespace MicraPro.SerialCommunication.Infrastructure;

public class SerialCommunicationInfrastructureOptions
{
    public static string SectionName { get; } =
        typeof(SerialCommunicationInfrastructureOptions).Namespace!.Replace('.', ':');
    public string SerialPort { get; set; } = string.Empty;
    public int BaudRate { get; set; }
}
