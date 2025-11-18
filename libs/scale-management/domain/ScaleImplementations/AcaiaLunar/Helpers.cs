namespace MicraPro.ScaleManagement.Domain.ScaleImplementations.AcaiaLunar;

public static class Helpers
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

    public static int MessageStart(byte[] buffer)
    {
        for (var i = 0; i < buffer.Length - 1; i++)
            if (buffer[i] == Header1 && buffer[i + 1] == Header2)
                return i;
        return -1;
    }
}
