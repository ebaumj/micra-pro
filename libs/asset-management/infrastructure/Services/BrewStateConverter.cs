using MicraPro.AssetManagement.Infrastructure.Interfaces;
using MicraPro.BrewByWeight.DataDefinition;
using MicraPro.BrewByWeight.DataDefinition.ValueObjects;

namespace MicraPro.AssetManagement.Infrastructure.Services;

public class BrewStateConverter : IBrewStateConverter
{
    private record BrewByWeightProcess(
        string BeanId,
        double InCupQuantity,
        double GrindSetting,
        double CoffeeQuantity,
        double TargetExtractionTime,
        string Spout
    )
    {
        public static BrewByWeightProcess FromDomain(IBrewProcess process) =>
            new(
                process.BeanId.ToString(),
                process.InCupQuantity,
                process.GrindSetting,
                process.CoffeeQuantity,
                process.TargetExtractionTime.TotalSeconds,
                process.Spout.ToString()
            );
    }

    private record BrewByTimeProcess(double TargetExtractionTime)
    {
        public static BrewByTimeProcess FromDomain(IBrewByTimeProcess process) =>
            new(process.ExtractionTime.TotalSeconds);
    }

    private record BrewByWeightData(BrewByWeightProcess ProcessData, object RuntimeData);

    private record BrewByTimeData(BrewByTimeProcess ProcessData, object RuntimeData);

    public object Convert(IBrewProcess process, BrewByWeightTracking.Started tracking) =>
        new BrewByWeightData(BrewByWeightProcess.FromDomain(process), new { });

    public object Convert(IBrewProcess process, BrewByWeightTracking.Running tracking) =>
        new BrewByWeightData(
            BrewByWeightProcess.FromDomain(process),
            new
            {
                tracking.Flow,
                tracking.TotalQuantity,
                TotalTime = tracking.TotalTime.TotalSeconds,
            }
        );

    public object Convert(IBrewProcess process, BrewByWeightTracking.Finished tracking) =>
        new BrewByWeightData(
            BrewByWeightProcess.FromDomain(process),
            new
            {
                tracking.AverageFlow,
                tracking.TotalQuantity,
                TotalTime = tracking.ExtractionTime.TotalSeconds,
            }
        );

    public object Convert(IBrewProcess process, BrewByWeightTracking.Cancelled tracking) =>
        new BrewByWeightData(
            BrewByWeightProcess.FromDomain(process),
            new
            {
                tracking.AverageFlow,
                tracking.TotalQuantity,
                TotalTime = tracking.TotalTime.TotalSeconds,
            }
        );

    public object Convert(IBrewProcess process, BrewByWeightTracking.Failed tracking) =>
        new BrewByWeightData(
            BrewByWeightProcess.FromDomain(process),
            new
            {
                tracking.AverageFlow,
                tracking.TotalQuantity,
                TotalTime = tracking.TotalTime.TotalSeconds,
                Error = tracking.Exception.GetType().Name,
            }
        );

    public object Convert(IBrewByTimeProcess process, BrewByTimeTracking.Started tracking) =>
        new BrewByTimeData(BrewByTimeProcess.FromDomain(process), new { });

    public object Convert(IBrewByTimeProcess process, BrewByTimeTracking.Running tracking) =>
        new BrewByTimeData(
            BrewByTimeProcess.FromDomain(process),
            new
            {
                TargetTime = tracking.TargetTime.TotalSeconds,
                TotalTime = tracking.TotalTime.TotalSeconds,
            }
        );

    public object Convert(IBrewByTimeProcess process, BrewByTimeTracking.Finished tracking) =>
        new BrewByTimeData(
            BrewByTimeProcess.FromDomain(process),
            new
            {
                TargetTime = tracking.TargetTime.TotalSeconds,
                TotalTime = tracking.ExtractionTime.TotalSeconds,
            }
        );

    public object Convert(IBrewByTimeProcess process, BrewByTimeTracking.Cancelled tracking) =>
        new BrewByTimeData(
            BrewByTimeProcess.FromDomain(process),
            new
            {
                TargetTime = tracking.TargetTime.TotalSeconds,
                TotalTime = tracking.TotalTime.TotalSeconds,
            }
        );

    public object Convert(IBrewByTimeProcess process, BrewByTimeTracking.Failed tracking) =>
        new BrewByTimeData(
            BrewByTimeProcess.FromDomain(process),
            new
            {
                TargetTime = tracking.TargetTime.TotalSeconds,
                TotalTime = tracking.TotalTime.TotalSeconds,
                Error = tracking.Exception.GetType().Name,
            }
        );
}
