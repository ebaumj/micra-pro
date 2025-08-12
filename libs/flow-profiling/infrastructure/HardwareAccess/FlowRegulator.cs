using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using MicraPro.FlowProfiling.Domain.HardwareAccess;
using MicraPro.SerialCommunication.DataDefinition;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace MicraPro.FlowProfiling.Infrastructure.HardwareAccess;

public class FlowRegulator(
    IOptions<FlowProfilingInfrastructureOptions> options,
    ISerialCommunicationService serialCommunicationService
) : IFlowPublisher, IFlowRegulator, IHostedService
{
    private readonly BehaviorSubject<double> _flowSubject = new(0);
    private IDisposable _subscription = Disposable.Empty;
    private bool _isAvailable;
    public IObservable<double> Flow => _flowSubject;
    public double CurrentFlow => _flowSubject.Value;

    public void SetFlow(double flow)
    {
        var nucleo = serialCommunicationService.GetNucleoBoard();
        if (nucleo == null)
            throw new Exception("Nucleo board not found");
        Observable.FromAsync(ct => nucleo.SetFlowAsync(flow, ct)).Subscribe();
    }

    public void StopRegulation()
    {
        var nucleo = serialCommunicationService.GetNucleoBoard();
        if (nucleo == null)
            throw new Exception("Nucleo board not found");
        Observable.FromAsync(ct => nucleo.StopFlowRegulationAsync(ct)).Subscribe();
    }

    public bool IsAvailable => options.Value.IsAvailable && _isAvailable;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        INucleoBoard? nucleo = null;
        while (nucleo == null && !cancellationToken.IsCancellationRequested)
        {
            nucleo = serialCommunicationService.GetNucleoBoard();
            await Task.Delay(200, cancellationToken);
        }
        if (nucleo != null)
        {
            _subscription = nucleo.Flow.Subscribe(isOn => _flowSubject.OnNext(isOn));
            _isAvailable = true;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _subscription.Dispose();
        return Task.CompletedTask;
    }
}
