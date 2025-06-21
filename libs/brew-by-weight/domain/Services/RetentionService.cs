using MicraPro.BrewByWeight.DataDefinition;
using MicraPro.BrewByWeight.Domain.Interfaces;
using MicraPro.BrewByWeight.Domain.StorageAccess;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.BrewByWeight.Domain.Services;

public class RetentionService(IServiceScopeFactory serviceScopeFactory) : IRetentionService
{
    private const double DefaultSingleSpoutRetention = 5;
    private const double DefaultDoubleSpoutRetention = 5;
    private const double DefaultNakedFilterRetention = 5;

    private async Task<double> CalculateRetentionFromNewestEntry(
        IReadOnlyCollection<FinishedProcessDb> entries,
        CancellationToken ct
    )
    {
        var latest = entries.MaxBy(e => e.Timestamp)!;
        var runtimeData = await serviceScopeFactory
            .CreateScope()
            .ServiceProvider.GetRequiredService<IBrewByWeightDbService>()
            .GetRuntimeDataAsync(latest.Id, ct);
        var totalQuantity = latest.TotalQuantity;
        var stopQuantity =
            runtimeData.LastOrDefault(r => r.TotalTime < latest.ExtractionTime)?.TotalQuantity
            ?? totalQuantity;
        if (stopQuantity > totalQuantity)
            return 0;
        return totalQuantity - stopQuantity;
    }

    public async Task<double> CalculateRetentionWeightAsync(
        Guid beanId,
        double inCupQuantity,
        double grindSetting,
        double coffeeQuantity,
        TimeSpan targetExtractionTime,
        IBrewByWeightService.Spout spout,
        CancellationToken ct
    )
    {
        var allProcessDataForSpout = (
            await serviceScopeFactory
                .CreateScope()
                .ServiceProvider.GetRequiredService<IBrewByWeightDbService>()
                .GetFinishedAsync(ct)
        )
            .Where(p => p.Spout == spout)
            .ToArray();
        if (allProcessDataForSpout.Length == 0)
            return spout switch
            {
                IBrewByWeightService.Spout.Single => DefaultSingleSpoutRetention,
                IBrewByWeightService.Spout.Double => DefaultDoubleSpoutRetention,
                _ => DefaultNakedFilterRetention,
            };
        var sameCoffeeQuantity = allProcessDataForSpout
            .Where(p => Math.Abs(p.CoffeeQuantity - coffeeQuantity) < 0.5)
            .ToArray();
        if (sameCoffeeQuantity.Length == 0)
            return await CalculateRetentionFromNewestEntry(allProcessDataForSpout, ct);
        var sameGrindSetting = sameCoffeeQuantity
            .Where(p => Math.Abs(p.GrindSetting - coffeeQuantity) < 0.2)
            .ToArray();
        if (sameGrindSetting.Length == 0)
            return await CalculateRetentionFromNewestEntry(sameCoffeeQuantity, ct);
        var sameTargetExtractionTime = sameGrindSetting
            .Where(p => p.TargetExtractionTime == targetExtractionTime)
            .ToArray();
        if (sameTargetExtractionTime.Length == 0)
            return await CalculateRetentionFromNewestEntry(sameGrindSetting, ct);
        var sameInCupQuantity = sameCoffeeQuantity
            .Where(p => Math.Abs(p.InCupQuantity - coffeeQuantity) < 0.5)
            .ToArray();
        if (sameInCupQuantity.Length == 0)
            return await CalculateRetentionFromNewestEntry(sameTargetExtractionTime, ct);
        var sameBeanId = sameCoffeeQuantity.Where(p => p.BeanId == beanId).ToArray();
        if (sameBeanId.Length == 0)
            return await CalculateRetentionFromNewestEntry(sameInCupQuantity, ct);
        return await CalculateRetentionFromNewestEntry(sameBeanId, ct);
    }
}
