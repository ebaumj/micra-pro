using System.Diagnostics;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using MicraPro.FlowProfiling.DataDefinition;
using MicraPro.FlowProfiling.DataDefinition.ValueObjects;
using MicraPro.FlowProfiling.Domain.HardwareAccess;
using MicraPro.FlowProfiling.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MicraPro.FlowProfiling.Domain.Services;

public class FlowProfilingService(
    IFlowPublisher flowPublisher,
    IFlowRampGeneratorService flowRampGeneratorService,
    ILogger<FlowProfilingService> logger
) : IFlowProfilingService
{
    private static readonly TimeSpan MaximumWaitTimeBeforeStop = TimeSpan.FromMinutes(1);

    private record ProcessDisposableToken(Guid ProcessId, IDisposable Disposable);

    private record FlowProfilingProcess(Guid ProcessId, IObservable<FlowProfileTracking> State)
        : IFlowProfilingProcess;

    private readonly BehaviorSubject<FlowProfilingState> _state = new(
        new FlowProfilingState.Idle()
    );
    private ProcessDisposableToken[] _processDisposables = [];

    public bool IsAvailable => flowPublisher.IsAvailable;
    public IObservable<FlowProfilingState> State => _state;

    public IFlowProfilingProcess RunFlowProfiling(
        double startFlow,
        IFlowProfilingService.FlowDataPoint[] dataPoints
    )
    {
        var processId = Guid.NewGuid();
        var subject = new ReplaySubject<FlowProfileTracking>();
        var allDataUpdates = new List<FlowProfileTracking.Running>();
        subject.OfType<FlowProfileTracking.Running>().Subscribe(s => allDataUpdates.Add(s));
        subject.OnNext(new FlowProfileTracking.Started());
        var subscription = Observable
            .FromAsync(async ct =>
            {
                try
                {
                    await RunFlowProfilingAsync(
                        startFlow,
                        dataPoints,
                        (flow, time) => subject.OnNext(new FlowProfileTracking.Running(flow, time)),
                        () => _state.OnNext(new FlowProfilingState.Running(processId)),
                        () => subject.OnNext(new FlowProfileTracking.ProfileDone()),
                        ct
                    );
                    var result = CalculateResult(allDataUpdates);
                    subject.OnNext(new FlowProfileTracking.Finished(result.Flow, result.Time));
                }
                catch (TaskCanceledException)
                {
                    var result = CalculateResult(allDataUpdates);
                    subject.OnNext(new FlowProfileTracking.Cancelled(result.Flow, result.Time));
                }
                catch (FlowProfileException exception)
                {
                    var result = CalculateResult(allDataUpdates);
                    subject.OnNext(
                        new FlowProfileTracking.Failed(exception, result.Flow, result.Time)
                    );
                }
                catch (Exception exception)
                {
                    var result = CalculateResult(allDataUpdates);
                    logger.LogError("Uncaught Exception during Flow Profiling: {e}", exception);
                    subject.OnNext(
                        new FlowProfileTracking.Failed(
                            new FlowProfileException.UnknownFlowProfilingError(),
                            result.Flow,
                            result.Time
                        )
                    );
                }
                finally
                {
                    subject.OnCompleted();
                    flowRampGeneratorService.StopFlowRamp();
                    _state.OnNext(new FlowProfilingState.Idle());
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
        return new FlowProfilingProcess(processId, subject);
    }

    public Task StopFlowProfilingProcess(Guid processId)
    {
        _processDisposables.FirstOrDefault(t => t.ProcessId == processId)?.Disposable.Dispose();
        return Task.CompletedTask;
    }

    private async Task RunFlowProfilingAsync(
        double startFlow,
        IFlowProfilingService.FlowDataPoint[] dataPoints,
        Action<double, TimeSpan> update,
        Action start,
        Action profileDone,
        CancellationToken ct
    )
    {
        if (_state.Value is not FlowProfilingState.Idle)
            throw new FlowProfileException.FlowProfilingServiceNotReady();
        start();
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        using var subscription = flowPublisher.Flow.Subscribe(f => update(f, stopwatch.Elapsed));
        var steps = dataPoints.ToList();
        steps.Sort((a, b) => a.Time.Ticks.CompareTo(b.Time.Ticks));
        flowRampGeneratorService.StartFlowRamp(startFlow, TimeSpan.Zero);
        var last = TimeSpan.Zero;
        foreach (var step in steps)
        {
            var stepTime = step.Time - last;
            last = step.Time;
            flowRampGeneratorService.StartFlowRamp(step.Flow, stepTime);
            await Task.Delay(stepTime, ct);
        }
        profileDone();
        try
        {
            await Task.Delay(MaximumWaitTimeBeforeStop, ct);
        }
        catch (TaskCanceledException)
        {
            // Stopped and profile done
        }
    }

    private (double Flow, TimeSpan Time) CalculateResult(
        List<FlowProfileTracking.Running> allDataUpdates
    )
    {
        allDataUpdates.Sort((a, b) => a.TotalTime.CompareTo(b.TotalTime));
        if (allDataUpdates.Count == 0)
            return (0, TimeSpan.Zero);
        var totalTime = allDataUpdates.Select(u => u.TotalTime).Last();
        var lastTime = TimeSpan.Zero;
        double accumulatedFlow = 0;
        foreach (var update in allDataUpdates)
        {
            accumulatedFlow += update.Flow * (update.TotalTime - lastTime).Ticks;
            lastTime = update.TotalTime;
        }
        return (accumulatedFlow / totalTime.Ticks, totalTime);
    }
}
