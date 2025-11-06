using MicraPro.Machine.DataDefinition.ValueObjects;

namespace MicraPro.Machine.DataProviderGraphQl.Types;

[GraphQLName("SmartStandby")]
public record SmartStandbyApi(TimeSpan Time, SmartStandbyApi.SmartStandbyMode Mode)
{
    public enum SmartStandbyMode
    {
        LastBrew,
        PowerOn,
        Off,
    }

    [GraphQLIgnore]
    public static SmartStandbyApi FromDomain(SmartStandby? standby) =>
        new(
            standby?.Time ?? TimeSpan.FromMinutes(60),
            standby == null ? SmartStandbyMode.Off : ConvertMode(standby.Mode)
        );

    [GraphQLIgnore]
    public SmartStandby? ToDomain() =>
        Mode == SmartStandbyMode.Off ? null : new SmartStandby(Time, ConvertMode(Mode));

    [GraphQLIgnore]
    private static SmartStandby.SmartStandbyMode ConvertMode(SmartStandbyMode mode) =>
        mode switch
        {
            SmartStandbyMode.PowerOn => SmartStandby.SmartStandbyMode.PowerOn,
            SmartStandbyMode.LastBrew => SmartStandby.SmartStandbyMode.LastBrew,
            _ => throw new Exception("Code not Working!"),
        };

    [GraphQLIgnore]
    private static SmartStandbyMode ConvertMode(SmartStandby.SmartStandbyMode mode) =>
        mode switch
        {
            SmartStandby.SmartStandbyMode.PowerOn => SmartStandbyMode.PowerOn,
            SmartStandby.SmartStandbyMode.LastBrew => SmartStandbyMode.LastBrew,
            _ => throw new Exception("Code not Working!"),
        };
}
