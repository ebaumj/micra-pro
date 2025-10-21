using MicraPro.BrewByWeight.DataDefinition;
using MicraPro.BrewByWeight.Domain.Interfaces;
using MicraPro.BrewByWeight.Domain.StorageAccess;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.BrewByWeight.Domain.Services;

public class RetentionService(IServiceScopeFactory serviceScopeFactory) : IRetentionService
{
    private const double DefaultSingleSpoutRetention = 5;
    private const double DefaultDoubleSpoutRetention = 5;
    private const double DefaultNakedFilterRetention = 1;

    private async Task<double> CalculateRetentionAsync(
        FinishedProcessDb entry,
        CancellationToken ct
    )
    {
        var runtimeData = await serviceScopeFactory
            .CreateScope()
            .ServiceProvider.GetRequiredService<IBrewByWeightDbService>()
            .GetRuntimeDataAsync(entry.Id, ct);
        var stopQuantity =
            runtimeData.LastOrDefault(r => r.TotalTime < entry.ExtractionTime)?.TotalQuantity
            ?? entry.TotalQuantity;
        if (stopQuantity > entry.TotalQuantity)
            return 0;
        return entry.TotalQuantity - stopQuantity;
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
        var processes = (
            await serviceScopeFactory
                .CreateScope()
                .ServiceProvider.GetRequiredService<IBrewByWeightDbService>()
                .GetFinishedAsync(ct)
        )
            .Where(p => p.Spout == spout)
            .ToArray();
        if (processes.Length == 0)
            return spout switch
            {
                IBrewByWeightService.Spout.Single => DefaultSingleSpoutRetention,
                IBrewByWeightService.Spout.Double => DefaultDoubleSpoutRetention,
                _ => DefaultNakedFilterRetention,
            };
        return await CalculateRetentionAsync(
            processes
                // Same Bean
                .WhereOrAll(p => p.BeanId == beanId)
                // Same Target Average Flow
                .WhereOrAll(p =>
                    Math.Abs(
                        p.InCupQuantity / p.TargetExtractionTime.TotalSeconds
                            - inCupQuantity / targetExtractionTime.TotalSeconds
                    ) < 0.1
                )
                // Same Ratio
                .WhereOrAll(p =>
                    Math.Abs(p.CoffeeQuantity / p.InCupQuantity - coffeeQuantity / inCupQuantity)
                    < 0.01
                )
                .MaxByOrFirst(p => p.Timestamp),
            ct
        );
    }
}
