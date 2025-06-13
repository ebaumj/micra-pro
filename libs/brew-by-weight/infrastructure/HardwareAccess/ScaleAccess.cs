using System.Reactive.Linq;
using System.Reactive.Subjects;
using MicraPro.BrewByWeight.Domain.HardwareAccess;
using Microsoft.Extensions.DependencyInjection;
using IScaleConnection = MicraPro.BrewByWeight.Domain.HardwareAccess.IScaleConnection;

namespace MicraPro.BrewByWeight.Infrastructure.HardwareAccess;

public class ScaleAccess(IServiceScopeFactory serviceScopeFactory) : IScaleAccess
{
    private class DummyScaleConnection : IScaleConnection
    {
        public DummyScaleConnection()
        {
            var rand = new Random();
            _simulation = Observable
                .Interval(TimeSpan.FromMilliseconds(100))
                .Select(d => new ScaleDataPoint(
                    0.18 * d,
                    d > 360 ? 0 : 0.2 * rand.NextDouble() + 1
                ));
            _subscription = _simulation.Subscribe(p => _subject.OnNext(p));
        }

        private IDisposable _subscription;
        private readonly Subject<ScaleDataPoint> _subject = new();
        private readonly IObservable<ScaleDataPoint> _simulation;
        public IObservable<ScaleDataPoint> Data => _subject;

        public Task TareAsync(CancellationToken ct)
        {
            _subscription.Dispose();
            _subscription = _simulation.Subscribe(p => _subject.OnNext(p));
            return Task.CompletedTask;
        }

        public Task DisconnectAsync(CancellationToken ct)
        {
            _subscription.Dispose();
            return Task.CompletedTask;
        }
    }

    public async Task<IScaleConnection> ConnectScaleAsync(Guid scaleId, CancellationToken ct)
    {
        //var scale = await serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<IScaleService>().GetScale(scaleId, ct);
        //return new ScaleConnection(await scale.ConnectAsync(ct));
        return new DummyScaleConnection();
    }

    private class ScaleConnection(
        MicraPro.ScaleManagement.DataDefinition.IScaleConnection connection
    ) : IScaleConnection
    {
        public IObservable<ScaleDataPoint> Data =>
            connection.Data.Select(d => new ScaleDataPoint(d.Weight, d.Flow));

        public Task TareAsync(CancellationToken ct) => connection.TareAsync(ct);

        public Task DisconnectAsync(CancellationToken ct) => connection.DisconnectAsync(ct);
    }
}
