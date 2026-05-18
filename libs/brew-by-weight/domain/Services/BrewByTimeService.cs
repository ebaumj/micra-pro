using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using MicraPro.BrewByWeight.DataDefinition;
using MicraPro.BrewByWeight.DataDefinition.ValueObjects;
using MicraPro.BrewByWeight.Domain.HardwareAccess;
using Microsoft.Extensions.Logging;

namespace MicraPro.BrewByWeight.Domain.Services;

public class BrewByTimeService(IPaddleAccess paddleAccess, ILogger<BrewByTimeService> logger)
    : IBrewByTimeService
{
    private record ProcessDisposableToken(Guid ProcessId, IDisposable Disposable);

    private record BrewProcess(Guid ProcessId, IObservable<BrewByTimeTracking> State)
        : IBrewByTimeProcess;

    private readonly BehaviorSubject<BrewByTimeState> _state = new(new BrewByTimeState.Idle());
    private ProcessDisposableToken[] _processDisposables = [];

    public IObservable<BrewByTimeState> State => _state.DistinctUntilChanged();

    private TimeSpan CalculateResult(ICollection<BrewByTimeTracking.Running> allDataUpdates) =>
        allDataUpdates.LastOrDefault()?.TotalTime ?? TimeSpan.Zero;

    public IBrewByTimeProcess RunBrewByTime(TimeSpan targetExtractionTime)
    {
        var processId = Guid.NewGuid();
        var subject = new ReplaySubject<BrewByTimeTracking>();
        var allDataUpdates = new List<BrewByTimeTracking.Running>();
        subject.OfType<BrewByTimeTracking.Running>().Subscribe(allDataUpdates.Add);
        subject.OnNext(new BrewByTimeTracking.Started());
        var subscription = Observable
            .FromAsync(async ct =>
            {
                try
                {
                    var extractionTime = await RunBrewByTimeAsync(
                        targetExtractionTime,
                        subject.OnNext,
                        () => _state.OnNext(new BrewByTimeState.Running(processId)),
                        ct
                    );
                    subject.OnNext(
                        new BrewByTimeTracking.Finished(targetExtractionTime, extractionTime)
                    );
                }
                catch (TaskCanceledException)
                {
                    var result = CalculateResult(allDataUpdates);
                    subject.OnNext(new BrewByTimeTracking.Cancelled(targetExtractionTime, result));
                }
                catch (BrewByWeightException exception)
                {
                    var result = CalculateResult(allDataUpdates);
                    subject.OnNext(
                        new BrewByTimeTracking.Failed(exception, targetExtractionTime, result)
                    );
                }
                catch (Exception exception)
                {
                    logger.LogError("Uncaught Exception during Brew By Weight: {e}", exception);
                    var result = CalculateResult(allDataUpdates);
                    subject.OnNext(
                        new BrewByTimeTracking.Failed(
                            new BrewByWeightException.UnknownError(),
                            targetExtractionTime,
                            result
                        )
                    );
                }
                finally
                {
                    subject.OnCompleted();
                    _state.OnNext(new BrewByTimeState.Idle());
                    await paddleAccess.SetBrewPaddleOnAsync(false, CancellationToken.None);
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

    private async Task<TimeSpan> RunBrewByTimeAsync(
        TimeSpan targetExtractionTime,
        Action<BrewByTimeTracking> updateState,
        Action start,
        CancellationToken ct
    )
    {
        if (_state.Value is not BrewByTimeState.Idle)
            throw new BrewByWeightException.BrewServiceNotReady();
        start();
        var stopwatch = new Stopwatch();
        try
        {
            stopwatch.Start();
            await paddleAccess.SetBrewPaddleOnAsync(true, ct);
            using var subscription = Observable
                .Interval(TimeSpan.FromMilliseconds(100))
                .Subscribe(_ =>
                    updateState(
                        new BrewByTimeTracking.Running(targetExtractionTime, stopwatch.Elapsed)
                    )
                );
            await Task.Delay(targetExtractionTime, ct);
            await paddleAccess.SetBrewPaddleOnAsync(false, ct);
            return stopwatch.Elapsed;
        }
        finally
        {
            stopwatch.Stop();
        }
    }

    public Task StopBrewProcess(Guid processId)
    {
        _processDisposables.FirstOrDefault(t => t.ProcessId == processId)?.Disposable.Dispose();
        return Task.CompletedTask;
    }
}
