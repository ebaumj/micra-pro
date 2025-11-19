using System.Reactive.Subjects;
using MicraPro.Cleaning.DataDefinition;
using MicraPro.Cleaning.DataDefinition.ValueObjects;

namespace MicraPro.Cleaning.DataProviderGraphQl.Services;

public class CleaningProcessContainerService
{
    private readonly BehaviorSubject<CleaningState> _stateSubject = new(
        new CleaningState.Finished(TimeSpan.Zero, 0)
    );
    private CancellationTokenSource _tokenSource = new();

    public IObservable<CleaningState> State => _stateSubject;

    public void StartCleaning(Func<CancellationToken, IObservable<CleaningState>> startAction)
    {
        _tokenSource.Cancel();
        _tokenSource = new CancellationTokenSource();
        startAction(_tokenSource.Token).Subscribe(s => _stateSubject.OnNext(s));
    }

    public void StopCleaning()
    {
        _tokenSource.Cancel();
    }
}
