using System.Reactive.Linq;
using System.Reactive.Subjects;
using MicraPro.Shared.Domain.WiredConnections;

namespace MicraPro.Shared.Infrastructure.WiredConnections;

public class BrewPaddleAccessDummy : IBrewPaddleAccess
{
    private readonly BehaviorSubject<bool> _isOn = new(false);

    public IObservable<bool> IsOn => _isOn.DistinctUntilChanged();

    public Task SetBrewPaddleOnAsync(bool isOn, CancellationToken ct)
    {
        Console.WriteLine(isOn ? "Paddle On" : "Paddle Off");
        _isOn.OnNext(isOn);
        return Task.CompletedTask;
    }
}
