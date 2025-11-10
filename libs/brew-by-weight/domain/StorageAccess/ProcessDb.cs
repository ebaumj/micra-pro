using MicraPro.BrewByWeight.DataDefinition;
using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.BrewByWeight.Domain.StorageAccess;

public abstract class ProcessDb(
    Guid id,
    DateTime timestamp,
    Guid beanId,
    double inCupQuantity,
    double grindSetting,
    double coffeeQuantity,
    TimeSpan targetExtractionTime,
    IBrewByWeightService.Spout spout
) : IEntity
{
    public Guid Id { get; } = id;
    public DateTime Timestamp { get; } = timestamp;
    public Guid BeanId { get; } = beanId;
    public double InCupQuantity { get; } = inCupQuantity;
    public double GrindSetting { get; } = grindSetting;
    public double CoffeeQuantity { get; } = coffeeQuantity;
    public TimeSpan TargetExtractionTime { get; } = targetExtractionTime;
    public IBrewByWeightService.Spout Spout { get; } = spout;
    public ICollection<ProcessRuntimeDataDb> RuntimeData { get; } =
        new List<ProcessRuntimeDataDb>();
}

public class FinishedProcessDb : ProcessDb
{
    public double AverageFlow { get; }
    public double TotalQuantity { get; }
    public TimeSpan ExtractionTime { get; }

    private FinishedProcessDb(
        Guid id,
        DateTime timestamp,
        Guid beanId,
        double inCupQuantity,
        double grindSetting,
        double coffeeQuantity,
        TimeSpan targetExtractionTime,
        IBrewByWeightService.Spout spout,
        double averageFlow,
        double totalQuantity,
        TimeSpan extractionTime
    )
        : base(
            id,
            timestamp,
            beanId,
            inCupQuantity,
            grindSetting,
            coffeeQuantity,
            targetExtractionTime,
            spout
        )
    {
        AverageFlow = averageFlow;
        TotalQuantity = totalQuantity;
        ExtractionTime = extractionTime;
    }

    public FinishedProcessDb(
        Guid beanId,
        double inCupQuantity,
        double grindSetting,
        double coffeeQuantity,
        TimeSpan targetExtractionTime,
        IBrewByWeightService.Spout spout,
        double averageFlow,
        double totalQuantity,
        TimeSpan extractionTime
    )
        : this(
            Guid.NewGuid(),
            DateTime.Now,
            beanId,
            inCupQuantity,
            grindSetting,
            coffeeQuantity,
            targetExtractionTime,
            spout,
            averageFlow,
            totalQuantity,
            extractionTime
        ) { }
}

public class CancelledProcessDb : ProcessDb
{
    public double AverageFlow { get; }
    public double TotalQuantity { get; }
    public TimeSpan TotalTime { get; }

    private CancelledProcessDb(
        Guid id,
        DateTime timestamp,
        Guid beanId,
        double inCupQuantity,
        double grindSetting,
        double coffeeQuantity,
        TimeSpan targetExtractionTime,
        IBrewByWeightService.Spout spout,
        double averageFlow,
        double totalQuantity,
        TimeSpan totalTime
    )
        : base(
            id,
            timestamp,
            beanId,
            inCupQuantity,
            grindSetting,
            coffeeQuantity,
            targetExtractionTime,
            spout
        )
    {
        AverageFlow = averageFlow;
        TotalQuantity = totalQuantity;
        TotalTime = totalTime;
    }

    public CancelledProcessDb(
        Guid beanId,
        double inCupQuantity,
        double grindSetting,
        double coffeeQuantity,
        TimeSpan targetExtractionTime,
        IBrewByWeightService.Spout spout,
        double averageFlow,
        double totalQuantity,
        TimeSpan totalTime
    )
        : this(
            Guid.NewGuid(),
            DateTime.Now,
            beanId,
            inCupQuantity,
            grindSetting,
            coffeeQuantity,
            targetExtractionTime,
            spout,
            averageFlow,
            totalQuantity,
            totalTime
        ) { }
}

public class FailedProcessDb : ProcessDb
{
    public double AverageFlow { get; }
    public double TotalQuantity { get; }
    public TimeSpan TotalTime { get; }
    public string ErrorType { get; }

    private FailedProcessDb(
        Guid id,
        DateTime timestamp,
        Guid beanId,
        double inCupQuantity,
        double grindSetting,
        double coffeeQuantity,
        TimeSpan targetExtractionTime,
        IBrewByWeightService.Spout spout,
        double averageFlow,
        double totalQuantity,
        TimeSpan totalTime,
        string errorType
    )
        : base(
            id,
            timestamp,
            beanId,
            inCupQuantity,
            grindSetting,
            coffeeQuantity,
            targetExtractionTime,
            spout
        )
    {
        AverageFlow = averageFlow;
        TotalQuantity = totalQuantity;
        TotalTime = totalTime;
        ErrorType = errorType;
    }

    public FailedProcessDb(
        Guid beanId,
        double inCupQuantity,
        double grindSetting,
        double coffeeQuantity,
        TimeSpan targetExtractionTime,
        IBrewByWeightService.Spout spout,
        double averageFlow,
        double totalQuantity,
        TimeSpan totalTime,
        string errorType
    )
        : this(
            Guid.NewGuid(),
            DateTime.Now,
            beanId,
            inCupQuantity,
            grindSetting,
            coffeeQuantity,
            targetExtractionTime,
            spout,
            averageFlow,
            totalQuantity,
            totalTime,
            errorType
        ) { }
}
