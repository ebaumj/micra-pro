using MicraPro.SerialCommunication.Domain.ValueObjects;
using MicraPro.SerialCommunication.Domain.ValueObjects.Messages;

namespace MicraPro.SerialCommunication.Domain.Services;

public class MessageConverterService : IMessageConverterService
{
    public NucleoState Convert(SetStateRequestResponse value) =>
        new(
            (double)value.Flow / 100,
            value.Paddle,
            value.RegulatorState == SetStateRequestResponse.RegulatorStateType.On
        );

    public SetStateRequestResponse Convert(NucleoState value) =>
        new(
            (ushort)Math.Round(value.Flow * 100),
            value.PaddleOn,
            value.FlowRegulationActive
                ? SetStateRequestResponse.RegulatorStateType.On
                : SetStateRequestResponse.RegulatorStateType.Open
        );
}
