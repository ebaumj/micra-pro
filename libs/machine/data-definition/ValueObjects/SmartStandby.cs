namespace MicraPro.Machine.DataDefinition.ValueObjects;

public record SmartStandby(TimeSpan Time, SmartStandby.SmartStandbyMode Mode)
{
    public enum SmartStandbyMode
    {
        LastBrew,
        PowerOn,
    }
}
