using MicraPro.SerialCommunication.Domain.ValueObjects;

namespace MicraPro.SerialCommunication.Domain.Services;

public interface INucleoStateService
{
    IObservable<NucleoState?> StateObservable { get; }
    NucleoState? State { get; }
    void SetState(NucleoState? state);

    NucleoState RequestedState { get; set; }
}
