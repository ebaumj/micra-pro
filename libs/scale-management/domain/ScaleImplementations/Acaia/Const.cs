namespace MicraPro.ScaleManagement.Domain.ScaleImplementations.Acaia;

public static class Const
{
    public const byte Header1 = 0xEF;
    public const byte Header2 = 0xDD;
    public const byte CommandTypeData = 12;
    public const byte MessageTypeHeartBeat = 0;
    public const byte MessageTypeTare = 4;
    public const byte MessageTypeWeightData = 5;
    public const byte MessageTypeWeightTimeData = 7;
    public const byte MessageTypeWeightTareData = 8;
    public const byte MessageTypeNotificationRequest = 12;
    public const byte MessageTypeIdentify = 11;

    public static readonly byte[] IdOldStyle =
    [
        0x2d,
        0x2d,
        0x2d,
        0x2d,
        0x2d,
        0x2d,
        0x2d,
        0x2d,
        0x2d,
        0x2d,
        0x2d,
        0x2d,
        0x2d,
        0x2d,
        0x2d,
    ];
    public static readonly byte[] IdPyxisStyle =
    [
        0x30,
        0x31,
        0x32,
        0x33,
        0x34,
        0x35,
        0x36,
        0x37,
        0x38,
        0x39,
        0x30,
        0x31,
        0x32,
        0x33,
        0x34,
    ];

    public static readonly byte[] NotificationRequest =
    [
        0, // weight
        1, // weight argument
        1, // battery
        2, // battery argument
        2, // timer
        5, // timer argument (number heartbeats between timer messages)
        3, // key
        4, // setting
    ];

    public static readonly TimeSpan HeartbeatIntervalOld = TimeSpan.FromSeconds(5);
    public static readonly TimeSpan HeartbeatIntervalPyxis = TimeSpan.FromSeconds(1);
}
