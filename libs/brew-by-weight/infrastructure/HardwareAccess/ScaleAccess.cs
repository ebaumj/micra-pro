using System.Reactive.Linq;
using System.Reactive.Subjects;
using MicraPro.BrewByWeight.DataDefinition.ValueObjects;
using MicraPro.BrewByWeight.Domain.HardwareAccess;
using MicraPro.ScaleManagement.DataDefinition;
using Microsoft.Extensions.DependencyInjection;
using IScaleConnection = MicraPro.BrewByWeight.Domain.HardwareAccess.IScaleConnection;

namespace MicraPro.BrewByWeight.Infrastructure.HardwareAccess;

public class ScaleAccess(IServiceScopeFactory serviceScopeFactory) : IScaleAccess
{
    public async Task<IScaleConnection> ConnectScaleAsync(CancellationToken ct)
    {
        var scale = await serviceScopeFactory
            .CreateScope()
            .ServiceProvider.GetRequiredService<IScaleService>()
            .GetScaleAsync(ct);
        if (scale == null)
            throw new BrewByWeightException.ScaleConnectionFailed();
        return new ScaleConnection(await scale.ConnectAsync(ct));
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
