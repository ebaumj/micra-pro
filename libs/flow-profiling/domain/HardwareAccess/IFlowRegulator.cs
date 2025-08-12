namespace MicraPro.FlowProfiling.Domain.HardwareAccess;

public interface IFlowRegulator
{
    double CurrentFlow { get; }
    void SetFlow(double flow);
    bool IsAvailable { get; }
}
