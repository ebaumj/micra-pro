namespace MicraPro.Shared.UtilsDotnet;

public class DisposableWithCallback(IDisposable disposable, Action onDispose) : IDisposable
{
    public void Dispose()
    {
        onDispose();
        disposable.Dispose();
    }
}
