using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using MicraPro.SerialCommunication.DataDefinition;
using MicraPro.Shared.Domain.WiredConnections;
using Microsoft.Extensions.Hosting;

namespace MicraPro.Shared.Infrastructure.WiredConnections;

public class BrewPaddleAccessSerial(ISerialCommunicationService serialCommunicationService)
    : IBrewPaddleAccess,
        IHostedService
{
    private readonly BehaviorSubject<bool> _brewPaddle = new(false);
    private IDisposable _subscription = Disposable.Empty;

    public IObservable<bool> IsOn => _brewPaddle.AsObservable();

    public Task SetBrewPaddleOnAsync(bool isOn, CancellationToken ct)
    {
        var nucleo = serialCommunicationService.GetNucleoBoard();
        if (nucleo == null)
            throw new Exception("Nucleo board not found");
        return nucleo.SetBrewPaddleAsync(isOn, ct);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _subscription = Observable.FromAsync(PollNucleoBoard).Subscribe();
        return Task.CompletedTask;
    }

    private async Task PollNucleoBoard(CancellationToken ct)
    {
        INucleoBoard? nucleo = null;
        while (nucleo == null && !ct.IsCancellationRequested)
        {
            nucleo = serialCommunicationService.GetNucleoBoard();
            await Task.Delay(200, ct);
        }
        if (nucleo != null)
            _subscription = nucleo.BrewPaddle.Subscribe(isOn => _brewPaddle.OnNext(isOn));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _subscription.Dispose();
        return Task.CompletedTask;
    }
}
