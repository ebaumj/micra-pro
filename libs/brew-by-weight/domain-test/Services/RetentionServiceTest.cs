using MicraPro.BrewByWeight.DataDefinition;
using MicraPro.BrewByWeight.DataDefinition.ValueObjects;
using MicraPro.BrewByWeight.Domain.Interfaces;
using MicraPro.BrewByWeight.Domain.Services;
using MicraPro.BrewByWeight.Domain.StorageAccess;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace MicraPro.BrewByWeight.Domain.Test.Services;

public class RetentionServiceTest
{
    public class DatabaseMock(DatabaseMock.Entry[] entries) : IBrewByWeightDbService
    {
        public record Entry(FinishedProcessDb Process, ProcessRuntimeDataDb[] RuntimeData);

        public Task StoreProcessAsync(
            Guid beanId,
            double inCupQuantity,
            double grindSetting,
            double coffeeQuantity,
            TimeSpan targetExtractionTime,
            IBrewByWeightService.Spout spout,
            IReadOnlyCollection<BrewByWeightTracking> tracking,
            CancellationToken ct
        ) => throw new NotImplementedException();

        public Task<IEnumerable<FinishedProcessDb>> GetFinishedAsync(CancellationToken ct) =>
            Task.FromResult(entries.Select(d => d.Process));

        public Task<IEnumerable<ProcessRuntimeDataDb>> GetRuntimeDataAsync(
            Guid processId,
            CancellationToken ct
        ) =>
            Task.FromResult<IEnumerable<ProcessRuntimeDataDb>>(
                entries.FirstOrDefault(i => i.Process.Id == processId)?.RuntimeData ?? []
            );
    }

    private static RetentionService CreateService(IBrewByWeightDbService dbService)
    {
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(m => m.GetService(typeof(IBrewByWeightDbService)))
            .Returns(dbService);
        var serviceScopeMock = new Mock<IServiceScope>();
        serviceScopeMock.Setup(m => m.ServiceProvider).Returns(serviceProviderMock.Object);
        var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
        serviceScopeFactoryMock.Setup(m => m.CreateScope()).Returns(serviceScopeMock.Object);
        return new RetentionService(serviceScopeFactoryMock.Object);
    }

    [Fact]
    public async Task DefaultsTes()
    {
        var service = CreateService(new DatabaseMock([]));
        Assert.InRange(
            await service.CalculateRetentionWeightAsync(
                Guid.Empty,
                0,
                0,
                0,
                TimeSpan.Zero,
                IBrewByWeightService.Spout.Single,
                CancellationToken.None
            ),
            4.99,
            5.01
        );
        Assert.InRange(
            await service.CalculateRetentionWeightAsync(
                Guid.Empty,
                0,
                0,
                0,
                TimeSpan.Zero,
                IBrewByWeightService.Spout.Double,
                CancellationToken.None
            ),
            4.99,
            5.01
        );
        Assert.InRange(
            await service.CalculateRetentionWeightAsync(
                Guid.Empty,
                0,
                0,
                0,
                TimeSpan.Zero,
                IBrewByWeightService.Spout.Naked,
                CancellationToken.None
            ),
            0.99,
            1.01
        );
    }

    private static readonly Guid BeanId1 = Guid.NewGuid();
    private static readonly Guid BeanId2 = Guid.NewGuid();

    private static DatabaseMock.Entry CreateEntry(
        Guid beanId,
        double inCupQuantity,
        double grindSetting,
        double coffeeQuantity,
        TimeSpan targetExtractionTime,
        IBrewByWeightService.Spout spout,
        double totalLiquid,
        double stopQuantity
    )
    {
        var process = new FinishedProcessDb(
            beanId,
            inCupQuantity,
            grindSetting,
            coffeeQuantity,
            targetExtractionTime,
            spout,
            1,
            totalLiquid,
            targetExtractionTime
        );
        return new DatabaseMock.Entry(
            process,
            [
                new ProcessRuntimeDataDb(
                    process.Id,
                    1,
                    stopQuantity - 2,
                    targetExtractionTime - TimeSpan.FromSeconds(2)
                ),
                new ProcessRuntimeDataDb(
                    process.Id,
                    1,
                    stopQuantity - 1,
                    targetExtractionTime - TimeSpan.FromSeconds(1)
                ),
                new ProcessRuntimeDataDb(
                    process.Id,
                    1,
                    stopQuantity,
                    targetExtractionTime - TimeSpan.FromMilliseconds(1)
                ),
                new ProcessRuntimeDataDb(
                    process.Id,
                    1,
                    stopQuantity + 1,
                    targetExtractionTime + TimeSpan.FromSeconds(1)
                ),
                new ProcessRuntimeDataDb(
                    process.Id,
                    1,
                    stopQuantity + 2,
                    targetExtractionTime + TimeSpan.FromSeconds(2)
                ),
            ]
        );
    }

    public static readonly TheoryData<
        Guid,
        double,
        double,
        double,
        TimeSpan,
        IBrewByWeightService.Spout,
        double,
        DatabaseMock.Entry[]
    > ProcessResults = new()
    {
        {
            BeanId1,
            40,
            18,
            20,
            TimeSpan.FromSeconds(25),
            IBrewByWeightService.Spout.Single,
            1.5,
            [
                CreateEntry(
                    BeanId1,
                    40,
                    18,
                    20,
                    TimeSpan.FromSeconds(25),
                    IBrewByWeightService.Spout.Single,
                    40,
                    30
                ),
                CreateEntry(
                    BeanId1,
                    40,
                    18,
                    20,
                    TimeSpan.FromSeconds(25),
                    IBrewByWeightService.Spout.Single,
                    40,
                    38.5
                ),
            ]
        },
        {
            BeanId1,
            40,
            18,
            20,
            TimeSpan.FromSeconds(25),
            IBrewByWeightService.Spout.Single,
            4,
            [
                CreateEntry(
                    BeanId2,
                    36,
                    15,
                    16,
                    TimeSpan.FromSeconds(28),
                    IBrewByWeightService.Spout.Single,
                    40,
                    36
                ),
                CreateEntry(
                    BeanId1,
                    40,
                    18,
                    20,
                    TimeSpan.FromSeconds(25),
                    IBrewByWeightService.Spout.Double,
                    40,
                    30
                ),
                CreateEntry(
                    BeanId1,
                    40,
                    18,
                    20,
                    TimeSpan.FromSeconds(25),
                    IBrewByWeightService.Spout.Naked,
                    40,
                    30
                ),
            ]
        },
        {
            BeanId2,
            40,
            18,
            20,
            TimeSpan.FromSeconds(25),
            IBrewByWeightService.Spout.Single,
            4,
            [
                CreateEntry(
                    BeanId2,
                    36,
                    15,
                    16,
                    TimeSpan.FromSeconds(28),
                    IBrewByWeightService.Spout.Single,
                    40,
                    36
                ),
                CreateEntry(
                    BeanId1,
                    40,
                    18,
                    20,
                    TimeSpan.FromSeconds(25),
                    IBrewByWeightService.Spout.Single,
                    40,
                    30
                ),
            ]
        },
        {
            BeanId1,
            40,
            18,
            20,
            TimeSpan.FromSeconds(20),
            IBrewByWeightService.Spout.Single,
            4,
            [
                CreateEntry(
                    BeanId1,
                    36,
                    15,
                    16,
                    TimeSpan.FromSeconds(18),
                    IBrewByWeightService.Spout.Single,
                    40,
                    36
                ),
                CreateEntry(
                    BeanId1,
                    40,
                    18,
                    20,
                    TimeSpan.FromSeconds(25),
                    IBrewByWeightService.Spout.Single,
                    40,
                    30
                ),
            ]
        },
        {
            BeanId1,
            40,
            18,
            20,
            TimeSpan.FromSeconds(20),
            IBrewByWeightService.Spout.Single,
            4,
            [
                CreateEntry(
                    BeanId1,
                    36,
                    15,
                    18,
                    TimeSpan.FromSeconds(18),
                    IBrewByWeightService.Spout.Single,
                    40,
                    36
                ),
                CreateEntry(
                    BeanId1,
                    40,
                    18,
                    19,
                    TimeSpan.FromSeconds(20),
                    IBrewByWeightService.Spout.Single,
                    40,
                    30
                ),
            ]
        },
    };

    [Theory]
    [MemberData(nameof(ProcessResults))]
    public async Task CalculateRetentionTest(
        Guid beanId,
        double inCupQuantity,
        double grindSetting,
        double coffeeQuantity,
        TimeSpan targetExtractionTime,
        IBrewByWeightService.Spout spout,
        double retentionTarget,
        DatabaseMock.Entry[] database
    )
    {
        Assert.InRange(
            await CreateService(new DatabaseMock(database))
                .CalculateRetentionWeightAsync(
                    beanId,
                    inCupQuantity,
                    grindSetting,
                    coffeeQuantity,
                    targetExtractionTime,
                    spout,
                    CancellationToken.None
                ),
            retentionTarget - 0.001,
            retentionTarget + 0.001
        );
    }
}
