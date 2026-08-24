using System.Reactive.Linq;
using MicraPro.AssetManagement.Domain.MachineAccess;
using MicraPro.AssetManagement.Infrastructure.Interfaces;
using MicraPro.Cleaning.DataDefinition;
using MicraPro.Cleaning.DataDefinition.ValueObjects;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.AssetManagement.Infrastructure.MachineAccess;

public class CleaningStatePublisher(
    IServiceScopeFactory serviceScopeFactory,
    ICleaningStateConverter cleaningStateConverter
) : ICleaningStatePublisher
{
    public IObservable<object> CleaningStarted =>
        SelectCleaningState<CleaningState.Started>()
            .Select(s => cleaningStateConverter.Convert(s.Cycles, s.State));
    public IObservable<object> CleaningUpdate =>
        SelectCleaningState<CleaningState.Running>()
            .Select(s => cleaningStateConverter.Convert(s.Cycles, s.State));
    public IObservable<object> CleaningFinished =>
        SelectCleaningState<CleaningState.Finished>()
            .Select(s => cleaningStateConverter.Convert(s.Cycles, s.State));
    public IObservable<object> CleaningStopped =>
        SelectCleaningState<CleaningState.Cancelled>()
            .Select(s => cleaningStateConverter.Convert(s.Cycles, s.State));
    public IObservable<object> CleaningError =>
        SelectCleaningState<CleaningState.Failed>()
            .Select(s => cleaningStateConverter.Convert(s.Cycles, s.State));

    private ICleaningService CleaningService =>
        serviceScopeFactory.CreateScope().ServiceProvider.GetRequiredService<ICleaningService>();

    private IObservable<(CleaningCycle[] Cycles, T State)> SelectCleaningState<T>()
        where T : CleaningState
    {
        return CleaningService
            .IsRunning.Where(r => r)
            .Select(_ =>
                Observable.FromAsync(async ct => await CleaningService.GetCleaningSequenceAsync(ct))
            )
            .Merge()
            .Select(sequence =>
            {
                var state = CleaningService.GetCleaningStateObservable();
                return state.Where(s => s is T).Cast<T>().Select(s => (sequence, s));
            })
            .Merge();
    }
}
