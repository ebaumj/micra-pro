namespace MicraPro.AssetManagement.Domain.MachineAccess;

public interface ICleaningStatePublisher
{
    IObservable<object> CleaningStarted { get; }
    IObservable<object> CleaningUpdate { get; }
    IObservable<object> CleaningFinished { get; }
    IObservable<object> CleaningStopped { get; }
    IObservable<object> CleaningError { get; }
}
