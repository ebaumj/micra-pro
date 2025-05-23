using System.Collections.Concurrent;

namespace MicraPro.Shared.UtilsDotnet;

public class LifecycleAwareObservable<T>(
    IObservable<T> observable,
    Action onFirstSubscribe,
    Action onLastDispose
) : IObservable<T>
{
    private ConcurrentBag<IObserver<T>> _observers = [];

    public IDisposable Subscribe(IObserver<T> observer)
    {
        if (_observers.IsEmpty)
            onFirstSubscribe();
        _observers.Add(observer);
        return new DisposableWithCallback(
            observable.Subscribe(observer),
            () =>
            {
                _observers = new ConcurrentBag<IObserver<T>>(
                    _observers.Where(o => !o.Equals(observer))
                );
                if (_observers.IsEmpty)
                    onLastDispose();
            }
        );
    }
}
