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
    private record ProcessHandle(IBrewByTimeProcess Process, IDisposable Disposable);

    private record BrewProcess(
        Guid ProcessId,
        IObservable<BrewByTimeTracking> State,
        TimeSpan ExtractionTime
    ) : IBrewByTimeProcess;

    private readonly BehaviorSubject<BrewByTimeState> _state = new(new BrewByTimeState.Idle());
    private ProcessHandle[] _processHandles = [];

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
        var process = new BrewProcess(processId, subject, targetExtractionTime);
        var subscription = Observable
            .FromAsync(async ct =>
            {
                try
                {
                    var extractionTime = await RunBrewByTimeAsync(
                        targetExtractionTime,
                        subject.OnNext,
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
                _processHandles = _processHandles
                    .Where(h => h.Process.ProcessId != processId)
                    .ToArray();
            });
        _processHandles = _processHandles
            .Append(new ProcessHandle(process, subscription))
            .ToArray();
        _state.OnNext(new BrewByTimeState.Running(processId));
        return process;
    }

    private async Task<TimeSpan> RunBrewByTimeAsync(
        TimeSpan targetExtractionTime,
        Action<BrewByTimeTracking> updateState,
        CancellationToken ct
    )
    {
        if (_state.Value is not BrewByTimeState.Idle)
            throw new BrewByWeightException.BrewServiceNotReady();
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

    public IBrewByTimeProcess? GetBrewProcess(Guid processId) =>
        _processHandles.FirstOrDefault(t => t.Process.ProcessId == processId)?.Process;

    public Task StopBrewProcess(Guid processId)
    {
        _processHandles.FirstOrDefault(t => t.Process.ProcessId == processId)?.Disposable.Dispose();
        return Task.CompletedTask;
    }
}
