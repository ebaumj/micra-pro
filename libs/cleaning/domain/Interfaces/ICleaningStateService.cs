namespace MicraPro.Cleaning.Domain.Interfaces;

public interface ICleaningStateService
{
    IObservable<bool> IsRunningObservable { get; }
    bool IsRunning { get; }
    void SetIsRunning(bool value);
}
