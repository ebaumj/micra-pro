namespace MicraPro.FlowProfiling.Domain.Interfaces;

public interface IFlowRampGeneratorService
{
    void StartFlowRamp(double targetFlow, TimeSpan duration);
    void StopFlowRamp();
}
