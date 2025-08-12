namespace MicraPro.FlowProfiling.Domain.HardwareAccess;

public interface IFlowPublisher
{
    IObservable<double> Flow { get; }
    bool IsAvailable { get; }
}
