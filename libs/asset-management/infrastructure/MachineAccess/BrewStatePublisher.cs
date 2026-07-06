using System.Reactive.Linq;
using MicraPro.AssetManagement.Domain.MachineAccess;
using MicraPro.AssetManagement.Infrastructure.Interfaces;
using MicraPro.BrewByWeight.DataDefinition;
using MicraPro.BrewByWeight.DataDefinition.ValueObjects;

namespace MicraPro.AssetManagement.Infrastructure.MachineAccess;

public class BrewStatePublisher(
    IBrewByWeightService brewByWeightService,
    IBrewByTimeService brewByTimeService,
    IBrewStateConverter brewStateConverter
) : IBrewStatePublisher
{
    public IObservable<object> BrewByWeightStarted =>
        SelectBrewState<BrewByWeightTracking.Started>()
            .Select(s => brewStateConverter.Convert(s.Process, s.Tracking));
    public IObservable<object> BrewByWeightUpdate =>
        SelectBrewState<BrewByWeightTracking.Running>()
            .Select(s => brewStateConverter.Convert(s.Process, s.Tracking));
    public IObservable<object> BrewByWeightFinished =>
        SelectBrewState<BrewByWeightTracking.Finished>()
            .Select(s => brewStateConverter.Convert(s.Process, s.Tracking));
    public IObservable<object> BrewByWeightStopped =>
        SelectBrewState<BrewByWeightTracking.Cancelled>()
            .Select(s => brewStateConverter.Convert(s.Process, s.Tracking));
    public IObservable<object> BrewByWeightError =>
        SelectBrewState<BrewByWeightTracking.Failed>()
            .Select(s => brewStateConverter.Convert(s.Process, s.Tracking));

    public IObservable<object> BrewByTimeStarted =>
        SelectBrewByTimeState<BrewByTimeTracking.Started>()
            .Select(s => brewStateConverter.Convert(s.Process, s.Tracking));
    public IObservable<object> BrewByTimeUpdate =>
        SelectBrewByTimeState<BrewByTimeTracking.Running>()
            .Select(s => brewStateConverter.Convert(s.Process, s.Tracking));
    public IObservable<object> BrewByTimeFinished =>
        SelectBrewByTimeState<BrewByTimeTracking.Finished>()
            .Select(s => brewStateConverter.Convert(s.Process, s.Tracking));
    public IObservable<object> BrewByTimeStopped =>
        SelectBrewByTimeState<BrewByTimeTracking.Cancelled>()
            .Select(s => brewStateConverter.Convert(s.Process, s.Tracking));
    public IObservable<object> BrewByTimeError =>
        SelectBrewByTimeState<BrewByTimeTracking.Failed>()
            .Select(s => brewStateConverter.Convert(s.Process, s.Tracking));

    private IObservable<(IBrewProcess Process, T Tracking)> SelectBrewState<T>()
        where T : BrewByWeightTracking =>
        brewByWeightService
            .State.Select(state =>
            {
                if (state is not BrewByWeightState.Running running)
                    return null;
                var process = brewByWeightService.GetBrewProcess(running.ProcessId);
                return process?.State.Select(tracking => (process, tracking));
            })
            .Where(s => s != null)!
            .Merge()
            .Where(s => s.tracking is T)
            .Select(s => (s.process, (T)s.tracking));

    private IObservable<(IBrewByTimeProcess Process, T Tracking)> SelectBrewByTimeState<T>()
        where T : BrewByTimeTracking =>
        brewByTimeService
            .State.Select(state =>
            {
                if (state is not BrewByTimeState.Running running)
                    return null;
                var process = brewByTimeService.GetBrewProcess(running.ProcessId);
                return process?.State.Select(tracking => (process, tracking));
            })
            .Where(s => s != null)!
            .Merge()
            .Where(s => s.tracking is T)
            .Select(s => (s.process, (T)s.tracking));
}
