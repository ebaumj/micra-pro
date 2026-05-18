using System.Reactive.Linq;
using MicraPro.BrewByWeight.DataDefinition;
using MicraPro.BrewByWeight.DataDefinition.ValueObjects;

namespace MicraPro.BrewByWeight.DataProviderGraphQl;

public class BrewByTimeProcessContainerService
{
    private static readonly TimeSpan BreProcessRetentionTime = TimeSpan.FromMinutes(1);

    private IBrewByTimeProcess[] _brewProcesses = [];

    public void AddBrewProcess(IBrewByTimeProcess brewProcess)
    {
        _brewProcesses = _brewProcesses.Append(brewProcess).ToArray();
        Observable
            .Timer(BreProcessRetentionTime)
            .Subscribe(_ =>
                _brewProcesses = _brewProcesses
                    .Where(p => p.ProcessId != brewProcess.ProcessId)
                    .ToArray()
            );
    }

    public IObservable<BrewByTimeTracking> GetTracker(Guid processId) =>
        _brewProcesses.FirstOrDefault(p => p.ProcessId == processId)?.State
        ?? throw new Exception("No Brew Process Found");
}
