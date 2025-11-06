using System.Reactive.Subjects;
using MicraPro.Machine.DataDefinition;

namespace MicraPro.Machine.DataProviderGraphQl.Services;

public class MachineScanContainerService(IMachineService service)
{
    private CancellationTokenSource _cancellation = new();
    private readonly BehaviorSubject<IMachineService.MachineScanResult[]> _results = new([]);
    private readonly BehaviorSubject<bool> _isScanning = new(false);
    public IObservable<IMachineService.MachineScanResult[]> Results => _results;
    public IObservable<bool> IsScanning => _isScanning;

    public void StartScanning()
    {
        if (_isScanning.Value)
            return;
        _cancellation = new CancellationTokenSource();
        _results.OnNext([]);
        _isScanning.OnNext(true);
        service
            .Scan(_cancellation.Token)
            .Subscribe(
                r => _results.OnNext(_results.Value.Append(r).ToArray()),
                _ => _isScanning.OnNext(false),
                () => _isScanning.OnNext(false)
            );
    }

    public void StopScanning()
    {
        _cancellation.Cancel();
    }
}
