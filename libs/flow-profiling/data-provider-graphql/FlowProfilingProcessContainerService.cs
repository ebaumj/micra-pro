using System.Reactive.Linq;
using MicraPro.FlowProfiling.DataDefinition;
using MicraPro.FlowProfiling.DataDefinition.ValueObjects;

namespace MicraPro.FlowProfiling.DataProviderGraphQl;

public class FlowProfilingProcessContainerService
{
    private static readonly TimeSpan FlowProfilingProcessRetentionTime = TimeSpan.FromMinutes(1);

    private IFlowProfilingProcess[] _flowProfilingProcesses = [];

    public void AddFlowProfilingProcess(IFlowProfilingProcess brewProcess)
    {
        _flowProfilingProcesses = _flowProfilingProcesses.Append(brewProcess).ToArray();
        Observable
            .Timer(FlowProfilingProcessRetentionTime)
            .Subscribe(_ =>
                _flowProfilingProcesses = _flowProfilingProcesses
                    .Where(p => p.ProcessId != brewProcess.ProcessId)
                    .ToArray()
            );
    }

    public IObservable<FlowProfileTracking> GetTracker(Guid processId) =>
        _flowProfilingProcesses.FirstOrDefault(p => p.ProcessId == processId)?.State
        ?? throw new Exception("No Flow Profiling Process Found");
}
