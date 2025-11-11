using System.Device.Gpio;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using MicraPro.Shared.Domain.WiredConnections;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MicraPro.Shared.Infrastructure.WiredConnections;

public class BrewPaddleAccess(
    IOptions<SharedInfrastructureOptions> configuration,
    ILogger<BrewPaddleAccess> logger
) : IHostedService, IBrewPaddleAccess
{
    private GpioPin? _pin;

    private readonly BehaviorSubject<bool> _isOn = new(false);

    public IObservable<bool> IsOn => _isOn.DistinctUntilChanged();

    public Task SetBrewPaddleOnAsync(bool isOn, CancellationToken ct)
    {
        if (_pin != null)
        {
            _pin.Write(isOn ? PinValue.High : PinValue.Low);
            _isOn.OnNext(isOn);
        }
        return Task.CompletedTask;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _pin =
                new GpioController().OpenPin(
                    configuration.Value.BrewPaddleRelaisGpio,
                    PinMode.Output
                ) ?? throw new Exception();
            _pin.Write(PinValue.Low);
            _isOn.OnNext(false);
        }
        catch (Exception e)
        {
            logger.LogError(
                "Failed to open GPIO {g}: {e}",
                configuration.Value.BrewPaddleRelaisGpio,
                e.Message
            );
        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            if (_pin != null)
            {
                _pin.Write(PinValue.Low);
                _pin.Close();
                _pin.Dispose();
                _isOn.OnNext(false);
            }
        }
        catch (Exception e)
        {
            logger.LogError(
                "Failed to close GPIO {g}: {e}",
                configuration.Value.BrewPaddleRelaisGpio,
                e.Message
            );
        }
        return Task.CompletedTask;
    }
}
