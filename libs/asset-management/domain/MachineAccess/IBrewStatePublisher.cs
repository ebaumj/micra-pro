namespace MicraPro.AssetManagement.Domain.MachineAccess;

public interface IBrewStatePublisher
{
    IObservable<object> BrewByWeightStarted { get; }
    IObservable<object> BrewByWeightUpdate { get; }
    IObservable<object> BrewByWeightFinished { get; }
    IObservable<object> BrewByWeightStopped { get; }
    IObservable<object> BrewByWeightError { get; }

    IObservable<object> BrewByTimeStarted { get; }
    IObservable<object> BrewByTimeUpdate { get; }
    IObservable<object> BrewByTimeFinished { get; }
    IObservable<object> BrewByTimeStopped { get; }
    IObservable<object> BrewByTimeError { get; }
}
