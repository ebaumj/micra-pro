using MicraPro.SerialCommunication.Domain.ValueObjects;
using MicraPro.SerialCommunication.Domain.ValueObjects.Messages;

namespace MicraPro.SerialCommunication.Domain.Services;

public interface IMessageConverterService
{
    NucleoState Convert(SetStateRequestResponse value);
    SetStateRequestResponse Convert(NucleoState value);
}
