namespace MicraPro.Shared.UtilsDotnet;

public class LifecycleAwareObservable<T>(
    IObservable<T> observable,
    Action onFirstSubscribe,
    Action onLastDispose
) : IObservable<T>
{
    private readonly HashSet<IObserver<T>> _observers = [];
    private readonly object _observersLock = new();

    public IDisposable Subscribe(IObserver<T> observer)
    {
        lock (_observersLock)
        {
            if (_observers.Count == 0)
                onFirstSubscribe();
            _observers.Add(observer);
        }
        return new DisposableWithCallback(
            observable.Subscribe(observer),
            () =>
            {
                lock (_observersLock)
                {
                    if (_observers.Remove(observer) && _observers.Count == 0)
                        onLastDispose();
                }
            }
        );
    }
}
