using MicraPro.SerialCommunication.DataDefinition;

namespace MicraPro.SerialCommunication.Domain.Services;

public class SerialCommunicationService(INucleoStateService nucleoStateService)
    : ISerialCommunicationService
{
    public INucleoBoard? GetNucleoBoard() =>
        nucleoStateService.State == null ? null : new NucleoBoard(nucleoStateService);
}
