using System.Reactive.Linq;

namespace MicraPro.ScaleManagement.DataProviderGraphQl;

public class ScanCancellationContainerService
{
    private CancellationTokenSource[] _tokenSources = [];
    private readonly object _tokenSourceLock = new();

    public void AddCancellationToken(CancellationTokenSource token, TimeSpan timeout)
    {
        lock (_tokenSourceLock)
        {
            _tokenSources = _tokenSources.Append(token).ToArray();
        }
        Observable
            .Timer(timeout)
            .Subscribe(_ =>
            {
                lock (_tokenSourceLock)
                {
                    _tokenSources = _tokenSources.Where(t => t != token).ToArray();
                }
            });
    }

    public void CancelAll()
    {
        lock (_tokenSourceLock)
        {
            foreach (var tokenSource in _tokenSources)
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
            }
            _tokenSources = [];
        }
    }
}
