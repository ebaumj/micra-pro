using System.Reactive.Subjects;
using MicraPro.SerialCommunication.Domain.ValueObjects;

namespace MicraPro.SerialCommunication.Domain.Services;

public class NucleoStateService : INucleoStateService
{
    private readonly BehaviorSubject<NucleoState?> _state = new(null);
    public IObservable<NucleoState?> StateObservable => _state;
    public NucleoState? State => _state.Value;

    public void SetState(NucleoState? state) => _state.OnNext(state);

    public NucleoState RequestedState { get; set; } = new(0, false, false);
}
