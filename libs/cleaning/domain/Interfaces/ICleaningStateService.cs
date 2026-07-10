using MicraPro.Cleaning.DataDefinition.ValueObjects;

namespace MicraPro.Cleaning.Domain.Interfaces;

public interface ICleaningStateService
{
    IObservable<bool> IsRunningObservable { get; }
    bool IsRunning { get; }
    void SetIsRunning(bool value);
    IObservable<CleaningState> CleaningStateObservable { get; }
    void SetCleaningStateObservable(IObservable<CleaningState> value);
}
