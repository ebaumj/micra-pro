using System.Device.Gpio;
using MicraPro.BrewByWeight.Domain.HardwareAccess;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MicraPro.BrewByWeight.Infrastructure.HardwareAccess;

public class PaddleAccess(
    IOptions<BrewByWeightInfrastructureOptions> configuration,
    ILogger<PaddleAccess> logger
) : IPaddleAccess, IHostedService
{
    private GpioPin? _pin;

    public Task SetBrewPaddleOnAsync(bool isOn, CancellationToken ct)
    {
        _pin?.Write(isOn ? PinValue.High : PinValue.Low);
        return Task.CompletedTask;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            _pin =
                new GpioController().OpenPin(configuration.Value.RelaisGpio, PinMode.Output)
                ?? throw new Exception();
        }
        catch (Exception e)
        {
            logger.LogError(
                "Failed to open GPIO {g}: {e}",
                configuration.Value.RelaisGpio,
                e.Message
            );
        }
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            _pin?.Write(PinValue.Low);
            _pin?.Close();
            _pin?.Dispose();
        }
        catch (Exception e)
        {
            logger.LogError(
                "Failed to close GPIO {g}: {e}",
                configuration.Value.RelaisGpio,
                e.Message
            );
        }
        return Task.CompletedTask;
    }
}
