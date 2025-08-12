namespace MicraPro.SerialCommunication.Domain.ValueObjects.Messages;

public record SetStateRequestResponse(
    ushort Flow,
    bool Paddle,
    SetStateRequestResponse.RegulatorStateType RegulatorState
)
{
    public enum RegulatorStateType
    {
        On,
        Open,
        Closed,
        Initializing,
    }

    public byte[] Serialize()
    {
        return
        [
            (byte)(Flow & 0x00FF),
            (byte)(Flow >> 8),
            (byte)(Paddle ? 1 : 0),
            (byte)RegulatorState,
        ];
    }

    public static SetStateRequestResponse Deserialize(byte[] data)
    {
        if (data.Length < 4)
            throw new ApplicationException("Invalid SetStateRequestResponse");
        return new SetStateRequestResponse(
            (ushort)(data[0] | (data[1] << 8)),
            data[2] > 0,
            (RegulatorStateType)data[3]
        );
    }
}
