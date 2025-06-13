using System.Reactive.Linq;

namespace MicraPro.ScaleManagement.DataProviderGraphQl;

public class ScanCancellationContainerService
{
    private CancellationTokenSource[] _tokenSources = [];

    public void AddCancellationToken(CancellationTokenSource token, TimeSpan timeout)
    {
        _tokenSources = _tokenSources.Append(token).ToArray();
        Observable
            .Timer(timeout)
            .Subscribe(_ => _tokenSources = _tokenSources.Where(t => t != token).ToArray());
    }

    public void CancelAll()
    {
        foreach (var tokenSource in _tokenSources)
            tokenSource.Cancel();
        _tokenSources = [];
    }
}
