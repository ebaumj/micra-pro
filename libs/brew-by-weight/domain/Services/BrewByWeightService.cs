using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using MicraPro.BrewByWeight.DataDefinition;
using MicraPro.BrewByWeight.DataDefinition.ValueObjects;
using MicraPro.BrewByWeight.Domain.HardwareAccess;
using MicraPro.BrewByWeight.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MicraPro.BrewByWeight.Domain.Services;

public class BrewByWeightService(
    IRetentionService retentionService,
    IPaddleAccess paddleAccess,
    IScaleAccess scaleAccess,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<BrewByWeightService> logger
) : IBrewByWeightService
{
    private static readonly TimeSpan MaximumDripTime = TimeSpan.FromSeconds(10);
    private static readonly TimeSpan WaitTimeAfterCupFull = TimeSpan.FromSeconds(3);

    private record ProcessDisposableToken(Guid ProcessId, IDisposable Disposable);

    private record BrewProcess(Guid ProcessId, IObservable<BrewByWeightTracking> State)
        : IBrewProcess;

    private readonly BehaviorSubject<BrewByWeightState> _state = new(new BrewByWeightState.Idle());
    private ProcessDisposableToken[] _processDisposables = [];

    private IBrewByWeightDbService BrewByWeightDbService =>
        serviceScopeFactory
            .CreateScope()
            .ServiceProvider.GetRequiredService<IBrewByWeightDbService>();

    public IObservable<BrewByWeightState> State => _state;

    private (double Flow, double Weight, TimeSpan Time) CalculateResultAsync(
        IReadOnlyCollection<BrewByWeightTracking.Running> dataUpdates
    )
    {
        var latestData = dataUpdates.LastOrDefault();
        return latestData is not null
            ? (dataUpdates.Average(x => x.Flow), latestData.TotalQuantity, latestData.TotalTime)
            : (0, 0, TimeSpan.Zero);
    }

    public IBrewProcess RunBrewByWeight(
        Guid beanId,
        Guid scaleId,
        double inCupQuantity,
        double grindSetting,
        double coffeeQuantity,
        TimeSpan targetExtractionTime,
        IBrewByWeightService.Spout spout
    )
    {
        var processId = Guid.NewGuid();
        var subject = new ReplaySubject<BrewByWeightTracking>();
        var allDataUpdates = new List<BrewByWeightTracking.Running>();
        subject.OfType<BrewByWeightTracking.Running>().Subscribe(s => allDataUpdates.Add(s));
        subject.OnNext(new BrewByWeightTracking.Started());
        var subscription = Observable
            .FromAsync(async ct =>
            {
                try
                {
                    var extractionTime = await RunBrewByWeightAsync(
                        beanId,
                        scaleId,
                        inCupQuantity,
                        grindSetting,
                        coffeeQuantity,
                        targetExtractionTime,
                        spout,
                        state => subject.OnNext(state),
                        ct
                    );
                    var result = CalculateResultAsync(allDataUpdates);
                    subject.OnNext(
                        new BrewByWeightTracking.Finished(
                            result.Flow,
                            result.Weight,
                            extractionTime
                        )
                    );
                }
                catch (TaskCanceledException)
                {
                    var result = CalculateResultAsync(allDataUpdates);
                    subject.OnNext(
                        new BrewByWeightTracking.Cancelled(result.Flow, result.Weight, result.Time)
                    );
                }
                catch (BrewByWeightException exception)
                {
                    var result = CalculateResultAsync(allDataUpdates);
                    subject.OnNext(
                        new BrewByWeightTracking.Failed(
                            exception,
                            result.Flow,
                            result.Weight,
                            result.Time
                        )
                    );
                }
                catch (Exception exception)
                {
                    logger.LogError("Uncaught Exception during Brew By Weight: {e}", exception);
                    var result = CalculateResultAsync(allDataUpdates);
                    subject.OnNext(
                        new BrewByWeightTracking.Failed(
                            new BrewByWeightException.UnknownError(),
                            result.Flow,
                            result.Weight,
                            result.Time
                        )
                    );
                }
                finally
                {
                    subject.OnCompleted();
                    await paddleAccess.SetBrewPaddleOnAsync(false, CancellationToken.None);
                    await BrewByWeightDbService.StoreProcessAsync(
                        beanId,
                        scaleId,
                        inCupQuantity,
                        grindSetting,
                        coffeeQuantity,
                        targetExtractionTime,
                        spout,
                        await subject.ToArray().ToTask(CancellationToken.None),
                        CancellationToken.None
                    );
                }
            })
            .Subscribe(_ =>
            {
                _processDisposables = _processDisposables
                    .Where(t => t.ProcessId != processId)
                    .ToArray();
            });
        _processDisposables = _processDisposables
            .Append(new ProcessDisposableToken(processId, subscription))
            .ToArray();
        return new BrewProcess(processId, subject);
    }

    private async Task<TimeSpan> RunBrewByWeightAsync(
        Guid beanId,
        Guid scaleId,
        double inCupQuantity,
        double grindSetting,
        double coffeeQuantity,
        TimeSpan targetExtractionTime,
        IBrewByWeightService.Spout spout,
        Action<BrewByWeightTracking> updateState,
        CancellationToken ct
    )
    {
        if (_state.Value is not BrewByWeightState.Idle)
            throw new BrewByWeightException.BrewServiceNotReady();
        var paddleOffWeight =
            inCupQuantity
            - await retentionService.CalculateRetentionWeightAsync(
                beanId,
                inCupQuantity,
                grindSetting,
                coffeeQuantity,
                targetExtractionTime,
                spout,
                ct
            );
        if (spout == IBrewByWeightService.Spout.Double)
            paddleOffWeight /= 2;
        IScaleConnection connection;
        var stopwatch = new Stopwatch();
        try
        {
            connection = await scaleAccess.ConnectScaleAsync(scaleId, ct);
        }
        catch
        {
            throw new BrewByWeightException.ScaleConnectionFailed();
        }
        try
        {
            await connection.TareAsync(ct);
            await connection.Data.TakeUntil(d => d.Weight is < 0.2 and > -0.2).ToTask(ct);
            stopwatch.Start();
            await paddleAccess.SetBrewPaddleOnAsync(true, ct);
            using var subscription = connection.Data.Subscribe(d =>
                updateState(
                    spout == IBrewByWeightService.Spout.Double
                        ? new BrewByWeightTracking.Running(
                            d.Flow * 2,
                            d.Weight * 2,
                            stopwatch.Elapsed
                        )
                        : new BrewByWeightTracking.Running(d.Flow, d.Weight, stopwatch.Elapsed)
                )
            );
            await connection.Data.TakeUntil(d => d.Weight >= paddleOffWeight).ToTask(ct);
            var extractionTime = stopwatch.Elapsed;
            await paddleAccess.SetBrewPaddleOnAsync(false, ct);
            var finishTime = stopwatch.Elapsed + MaximumDripTime;
            await connection
                .Data.TakeUntil(d => d.Flow < 0.1 || stopwatch.Elapsed > finishTime)
                .ToTask(ct);
            await Task.Delay(WaitTimeAfterCupFull, ct);
            return extractionTime;
        }
        finally
        {
            stopwatch.Stop();
            Observable
                .FromAsync(async t => await connection.DisconnectAsync(t))
                .Subscribe(_ => { });
        }
    }

    public Task StopBrewProcess(Guid processId)
    {
        _processDisposables.FirstOrDefault(t => t.ProcessId == processId)?.Disposable.Dispose();
        return Task.CompletedTask;
    }
}
