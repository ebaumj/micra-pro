namespace MicraPro.SerialCommunication.DataDefinition;

public interface INucleoBoard
{
    Task SetBrewPaddleAsync(bool on, CancellationToken ct);
    Task SetFlowAsync(double flow, CancellationToken ct);
    Task StopFlowRegulationAsync(CancellationToken ct);

    IObservable<bool> BrewPaddle { get; }
    IObservable<double> Flow { get; }
}
