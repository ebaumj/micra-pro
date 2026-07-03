using System.Reactive.Linq;
using System.Reactive.Subjects;
using MicraPro.Cleaning.DataDefinition.ValueObjects;
using MicraPro.Cleaning.Domain.Interfaces;

namespace MicraPro.Cleaning.Domain.Services;

public class CleaningStateService : ICleaningStateService
{
    private readonly BehaviorSubject<bool> _isRunning = new(false);
    private IObservable<CleaningState> _stateObservable = Observable.Empty<CleaningState>();

    public IObservable<bool> IsRunningObservable => _isRunning.DistinctUntilChanged();
    public bool IsRunning => _isRunning.Value;

    public void SetIsRunning(bool value) => _isRunning.OnNext(value);

    public IObservable<CleaningState> CleaningStateObservable => _stateObservable;

    public void SetCleaningStateObservable(IObservable<CleaningState> value) =>
        _stateObservable = value;
}
