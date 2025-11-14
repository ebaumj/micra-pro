using System.Reactive.Linq;
using System.Reactive.Subjects;
using MicraPro.Cleaning.DataDefinition;
using MicraPro.Cleaning.DataDefinition.ValueObjects;
using MicraPro.Cleaning.Domain.HardwareAccess;
using MicraPro.Cleaning.Domain.Interfaces;
using MicraPro.Cleaning.Domain.StorageAccess;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.Cleaning.Domain.Services;

public class CleaningService(
    ICleaningRepository repository,
    IBrewPaddle brewPaddle,
    ICleaningStateService stateService,
    ICleaningDefaultsProvider cleaningDefaultsProvider,
    IServiceScopeFactory scopeFactory
) : ICleaningService
{
    private static readonly TimeSpan UpdateInterval = TimeSpan.FromMilliseconds(200);

    public Task<CleaningCycle[]> GetCleaningSequenceAsync(CancellationToken ct) =>
        repository.GetCleaningSequenceAsync(ct);

    public Task SetCleaningSequenceAsync(CleaningCycle[] sequence, CancellationToken ct) =>
        repository.SetCleaningSequenceAsync(sequence, ct);

    public Task ResetCleaningSequenceAsync(CancellationToken ct) =>
        repository.SetCleaningSequenceAsync(cleaningDefaultsProvider.DefaultSequence, ct);

    public Task<TimeSpan> GetCleaningIntervalAsync(CancellationToken ct) =>
        repository.GetCleaningIntervalAsync(ct);

    public Task SetCleaningIntervalAsync(TimeSpan interval, CancellationToken ct) =>
        repository.SetCleaningIntervalAsync(interval, ct);

    public Task<DateTime> GetLastCleaningTimeAsync(CancellationToken ct) =>
        repository.GetLastCleaningTimeAsync(ct);

    public IObservable<CleaningState> StartCleaning(CancellationToken ct)
    {
        if (stateService.IsRunning)
            throw new Exception("State is already running");
        var subject = new ReplaySubject<CleaningState>();
        subject.OnNext(new CleaningState.Started());
        stateService.SetIsRunning(true);
        Observable.FromAsync(() => CleaningProcessAsync(subject, ct)).Subscribe();
        return subject.AsObservable();
    }

    private async Task CleaningProcessAsync(IObserver<CleaningState> observer, CancellationToken ct)
    {
        var startTime = DateTime.Now;
        var cycleStartTime = DateTime.Now;
        var cycle = 0;
        using var subscription = Observable
            .Interval(UpdateInterval)
            .Subscribe(_ =>
            {
                var now = DateTime.Now;
                // ReSharper disable line AccessToModifiedClosure
                observer.OnNext(
                    new CleaningState.Running(now - startTime, cycle, now - cycleStartTime)
                );
            });
        try
        {
            ct.ThrowIfCancellationRequested();
            var sequence = await scopeFactory
                .CreateScope()
                .ServiceProvider.GetRequiredService<ICleaningRepository>()
                .GetCleaningSequenceAsync(ct);
            foreach (var item in sequence)
            {
                cycleStartTime = DateTime.Now;
                await brewPaddle.SetPaddleAsync(true, ct);
                await Task.Delay(item.PaddleOnTime, ct);
                await brewPaddle.SetPaddleAsync(false, ct);
                await Task.Delay(item.PaddleOffTime, ct);
                cycle++;
            }
            var now = DateTime.Now;
            await scopeFactory
                .CreateScope()
                .ServiceProvider.GetRequiredService<ICleaningRepository>()
                .SetLastCleaningTimeAsync(now, ct);
            observer.OnNext(new CleaningState.Finished(now - startTime, cycle));
        }
        catch (OperationCanceledException)
        {
            observer.OnNext(new CleaningState.Cancelled(DateTime.Now - startTime, cycle));
        }
        catch (Exception e)
        {
            observer.OnNext(new CleaningState.Failed(DateTime.Now - startTime, cycle));
            Console.WriteLine(e);
        }
        finally
        {
            observer.OnCompleted();
            stateService.SetIsRunning(false);
            try
            {
                await brewPaddle.SetPaddleAsync(false, ct);
            }
            catch (Exception e)
            {
                // log exception
            }
        }
    }

    public IObservable<bool> IsRunning => stateService.IsRunningObservable;
}
