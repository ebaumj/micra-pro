namespace MicraPro.BrewByWeight.DataDefinition.ValueObjects;

public abstract record BrewByWeightHistoryEntry(
    Guid Id,
    DateTime Timestamp,
    Guid BeanId,
    Guid ScaleId,
    double InCupQuantity,
    double GrindSetting,
    double CoffeeQuantity,
    TimeSpan TargetExtractionTime,
    IBrewByWeightService.Spout Spout,
    double AverageFlow,
    double TotalQuantity,
    BrewByWeightHistoryRuntimeData[] RuntimeData
)
{
    public record ProcessFinished(
        Guid Id,
        DateTime Timestamp,
        Guid BeanId,
        Guid ScaleId,
        double InCupQuantity,
        double GrindSetting,
        double CoffeeQuantity,
        TimeSpan TargetExtractionTime,
        IBrewByWeightService.Spout Spout,
        double AverageFlow,
        double TotalQuantity,
        BrewByWeightHistoryRuntimeData[] RuntimeData,
        TimeSpan ExtractionTime
    )
        : BrewByWeightHistoryEntry(
            Id,
            Timestamp,
            BeanId,
            ScaleId,
            InCupQuantity,
            GrindSetting,
            CoffeeQuantity,
            TargetExtractionTime,
            Spout,
            AverageFlow,
            TotalQuantity,
            RuntimeData
        );

    public record ProcessCancelled(
        Guid Id,
        DateTime Timestamp,
        Guid BeanId,
        Guid ScaleId,
        double InCupQuantity,
        double GrindSetting,
        double CoffeeQuantity,
        TimeSpan TargetExtractionTime,
        IBrewByWeightService.Spout Spout,
        double AverageFlow,
        double TotalQuantity,
        BrewByWeightHistoryRuntimeData[] RuntimeData,
        TimeSpan TotalTime
    )
        : BrewByWeightHistoryEntry(
            Id,
            Timestamp,
            BeanId,
            ScaleId,
            InCupQuantity,
            GrindSetting,
            CoffeeQuantity,
            TargetExtractionTime,
            Spout,
            AverageFlow,
            TotalQuantity,
            RuntimeData
        );

    public record ProcessFailed(
        Guid Id,
        DateTime Timestamp,
        Guid BeanId,
        Guid ScaleId,
        double InCupQuantity,
        double GrindSetting,
        double CoffeeQuantity,
        TimeSpan TargetExtractionTime,
        IBrewByWeightService.Spout Spout,
        double AverageFlow,
        double TotalQuantity,
        BrewByWeightHistoryRuntimeData[] RuntimeData,
        TimeSpan TotalTime,
        string ErrorType
    )
        : BrewByWeightHistoryEntry(
            Id,
            Timestamp,
            BeanId,
            ScaleId,
            InCupQuantity,
            GrindSetting,
            CoffeeQuantity,
            TargetExtractionTime,
            Spout,
            AverageFlow,
            TotalQuantity,
            RuntimeData
        );
}
