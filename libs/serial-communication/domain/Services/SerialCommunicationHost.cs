using System.Reactive.Disposables;
using System.Reactive.Linq;
using MicraPro.SerialCommunication.Domain.HardwareAccess;
using MicraPro.SerialCommunication.Domain.ValueObjects.Messages;
using Microsoft.Extensions.Hosting;

namespace MicraPro.SerialCommunication.Domain.Services;

public class SerialCommunicationHost(
    INucleoStateService nucleoStateService,
    ISerialDataService serialDataService,
    IMessageConverterService messageConverterService
) : IHostedService
{
    private static readonly TimeSpan PollPeriod = TimeSpan.FromMilliseconds(200);

    private IDisposable _subscription = Disposable.Empty;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var responseReceived = false;
        _subscription = new CompositeDisposable(
            serialDataService
                .Received.Select(MessageV1.Deserialize)
                .Where(m => m.Type == MessageV1.MessageType.SetStateResponse)
                .Select(m => SetStateRequestResponse.Deserialize(m.Payload))
                .Subscribe(state =>
                {
                    nucleoStateService.SetState(messageConverterService.Convert(state));
                    responseReceived = true;
                }),
            Observable
                .Interval(PollPeriod)
                .Select(_ =>
                {
                    return Observable.FromAsync(ct =>
                        serialDataService.SendAsync(
                            messageConverterService
                                .Convert(nucleoStateService.RequestedState)
                                .Serialize(),
                            ct
                        )
                    );
                })
                .Merge()
                .Subscribe(_ =>
                {
                    if (!responseReceived)
                        nucleoStateService.SetState(null);
                    responseReceived = false;
                })
        );
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _subscription.Dispose();
        return Task.CompletedTask;
    }
}
