using System.Reactive.Linq;
using MicraPro.SerialCommunication.DataDefinition;

namespace MicraPro.SerialCommunication.Domain.Services;

public class NucleoBoard(INucleoStateService nucleoStateService) : INucleoBoard
{
    public Task SetBrewPaddleAsync(bool on, CancellationToken ct)
    {
        nucleoStateService.RequestedState = nucleoStateService.RequestedState with
        {
            PaddleOn = on,
        };
        return Task.CompletedTask;
    }

    public Task SetFlowAsync(double flow, CancellationToken ct)
    {
        nucleoStateService.RequestedState = nucleoStateService.RequestedState with
        {
            Flow = flow,
            FlowRegulationActive = true,
        };
        return Task.CompletedTask;
    }

    public Task StopFlowRegulationAsync(CancellationToken ct)
    {
        nucleoStateService.RequestedState = nucleoStateService.RequestedState with
        {
            FlowRegulationActive = false,
        };
        return Task.CompletedTask;
    }

    public IObservable<bool> BrewPaddle =>
        nucleoStateService.StateObservable.Where(s => s != null).Select(s => s!.PaddleOn);
    public IObservable<double> Flow =>
        nucleoStateService.StateObservable.Where(s => s != null).Select(s => s!.Flow);
}
