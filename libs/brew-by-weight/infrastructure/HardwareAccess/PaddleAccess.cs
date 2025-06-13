using MicraPro.BrewByWeight.Domain.HardwareAccess;

namespace MicraPro.BrewByWeight.Infrastructure.HardwareAccess;

public class PaddleAccess : IPaddleAccess
{
    public Task SetBrewPaddleOnAsync(bool isOn, CancellationToken ct)
    {
        Console.WriteLine(isOn ? "Paddle On" : "Paddle Off");
        return Task.CompletedTask;
    }
}
