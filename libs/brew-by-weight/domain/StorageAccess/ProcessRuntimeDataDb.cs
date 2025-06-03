using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.BrewByWeight.Domain.StorageAccess;

public class ProcessRuntimeDataDb : IEntity
{
    public Guid Id { get; }
    public Guid ProcessId { get; }
    public double Flow { get; }
    public double TotalQuantity { get; }
    public TimeSpan TotalTime { get; }

    public ProcessDb Process { get; init; } = null!;

    public ProcessRuntimeDataDb(
        Guid processId,
        double flow,
        double totalQuantity,
        TimeSpan totalTime
    )
        : this(Guid.NewGuid(), processId, flow, totalQuantity, totalTime) { }

    private ProcessRuntimeDataDb(
        Guid id,
        Guid processId,
        double flow,
        double totalQuantity,
        TimeSpan totalTime
    )
    {
        Id = id;
        ProcessId = processId;
        Flow = flow;
        TotalQuantity = totalQuantity;
        TotalTime = totalTime;
    }
}
