using MicraPro.BrewByWeight.DataDefinition;
using MicraPro.BrewByWeight.DataDefinition.ValueObjects;
using MicraPro.BrewByWeight.Domain.Services;
using MicraPro.BrewByWeight.Domain.StorageAccess;
using Microsoft.Extensions.Logging;
using Moq;

namespace MicraPro.BrewByWeight.Domain.Test.Services;

public class BrewByWeightHistoryServiceTest
{
    private static (
        ProcessDb DbProcess,
        ProcessRuntimeDataDb[] DbRuntime,
        BrewByWeightHistoryEntry Data
    ) CreateFinishedHistoryEntry(
        Guid beanId,
        double inCupQuantity,
        double grindSetting,
        double coffeeQuantity,
        TimeSpan targetExtractionTime,
        IBrewByWeightService.Spout spout
    )
    {
        var p = new FinishedProcessDb(
            beanId,
            inCupQuantity,
            grindSetting,
            coffeeQuantity,
            targetExtractionTime,
            spout,
            inCupQuantity / targetExtractionTime.TotalSeconds,
            inCupQuantity,
            targetExtractionTime
        );
        ProcessRuntimeDataDb[] runtimeData =
        [
            new(
                p.Id,
                inCupQuantity / targetExtractionTime.TotalSeconds,
                inCupQuantity / 5,
                targetExtractionTime / 5
            ),
            new(
                p.Id,
                inCupQuantity / targetExtractionTime.TotalSeconds,
                inCupQuantity * 2 / 5,
                targetExtractionTime * 2 / 5
            ),
            new(
                p.Id,
                inCupQuantity / targetExtractionTime.TotalSeconds,
                inCupQuantity * 3 / 5,
                targetExtractionTime * 3 / 5
            ),
            new(
                p.Id,
                inCupQuantity / targetExtractionTime.TotalSeconds,
                inCupQuantity * 4 / 5,
                targetExtractionTime * 4 / 5
            ),
            new(
                p.Id,
                inCupQuantity / targetExtractionTime.TotalSeconds,
                inCupQuantity,
                targetExtractionTime
            ),
        ];
        var list = runtimeData
            .Select(db => new BrewByWeightHistoryRuntimeData(
                db.Flow,
                db.TotalQuantity,
                db.TotalTime
            ))
            .ToList();
        list.Sort((a, b) => a.TotalTime.CompareTo(b.TotalTime));
        var data = new BrewByWeightHistoryEntry.ProcessFinished(
            p.Id,
            p.Timestamp,
            p.BeanId,
            p.InCupQuantity,
            p.GrindSetting,
            p.CoffeeQuantity,
            p.TargetExtractionTime,
            p.Spout,
            p.AverageFlow,
            p.TotalQuantity,
            list.ToArray(),
            p.ExtractionTime
        );
        return (p, runtimeData, data);
    }

    private static (
        ProcessDb DbProcess,
        ProcessRuntimeDataDb[] DbRuntime,
        BrewByWeightHistoryEntry Data
    ) CreateCancelledHistoryEntry(
        Guid beanId,
        double inCupQuantity,
        double grindSetting,
        double coffeeQuantity,
        TimeSpan targetExtractionTime,
        IBrewByWeightService.Spout spout
    )
    {
        var p = new CancelledProcessDb(
            beanId,
            inCupQuantity,
            grindSetting,
            coffeeQuantity,
            targetExtractionTime,
            spout,
            inCupQuantity / targetExtractionTime.TotalSeconds,
            inCupQuantity,
            targetExtractionTime
        );
        ProcessRuntimeDataDb[] runtimeData =
        [
            new(
                p.Id,
                inCupQuantity / targetExtractionTime.TotalSeconds,
                inCupQuantity / 5,
                targetExtractionTime / 5
            ),
            new(
                p.Id,
                inCupQuantity / targetExtractionTime.TotalSeconds,
                inCupQuantity * 2 / 5,
                targetExtractionTime * 2 / 5
            ),
            new(
                p.Id,
                inCupQuantity / targetExtractionTime.TotalSeconds,
                inCupQuantity * 3 / 5,
                targetExtractionTime * 3 / 5
            ),
            new(
                p.Id,
                inCupQuantity / targetExtractionTime.TotalSeconds,
                inCupQuantity * 4 / 5,
                targetExtractionTime * 4 / 5
            ),
            new(
                p.Id,
                inCupQuantity / targetExtractionTime.TotalSeconds,
                inCupQuantity,
                targetExtractionTime
            ),
        ];
        var list = runtimeData
            .Select(db => new BrewByWeightHistoryRuntimeData(
                db.Flow,
                db.TotalQuantity,
                db.TotalTime
            ))
            .ToList();
        list.Sort((a, b) => a.TotalTime.CompareTo(b.TotalTime));
        var data = new BrewByWeightHistoryEntry.ProcessCancelled(
            p.Id,
            p.Timestamp,
            p.BeanId,
            p.InCupQuantity,
            p.GrindSetting,
            p.CoffeeQuantity,
            p.TargetExtractionTime,
            p.Spout,
            p.AverageFlow,
            p.TotalQuantity,
            list.ToArray(),
            p.TotalTime
        );
        return (p, runtimeData, data);
    }

    private static (
        ProcessDb DbProcess,
        ProcessRuntimeDataDb[] DbRuntime,
        BrewByWeightHistoryEntry Data
    ) CreateFailedHistoryEntry(
        Guid beanId,
        double inCupQuantity,
        double grindSetting,
        double coffeeQuantity,
        TimeSpan targetExtractionTime,
        IBrewByWeightService.Spout spout,
        string errorType
    )
    {
        var p = new FailedProcessDb(
            beanId,
            inCupQuantity,
            grindSetting,
            coffeeQuantity,
            targetExtractionTime,
            spout,
            inCupQuantity / targetExtractionTime.TotalSeconds,
            inCupQuantity,
            targetExtractionTime,
            errorType
        );
        ProcessRuntimeDataDb[] runtimeData =
        [
            new(
                p.Id,
                inCupQuantity / targetExtractionTime.TotalSeconds,
                inCupQuantity / 5,
                targetExtractionTime / 5
            ),
            new(
                p.Id,
                inCupQuantity / targetExtractionTime.TotalSeconds,
                inCupQuantity * 2 / 5,
                targetExtractionTime * 2 / 5
            ),
            new(
                p.Id,
                inCupQuantity / targetExtractionTime.TotalSeconds,
                inCupQuantity * 3 / 5,
                targetExtractionTime * 3 / 5
            ),
            new(
                p.Id,
                inCupQuantity / targetExtractionTime.TotalSeconds,
                inCupQuantity * 4 / 5,
                targetExtractionTime * 4 / 5
            ),
            new(
                p.Id,
                inCupQuantity / targetExtractionTime.TotalSeconds,
                inCupQuantity,
                targetExtractionTime
            ),
        ];
        var list = runtimeData
            .Select(db => new BrewByWeightHistoryRuntimeData(
                db.Flow,
                db.TotalQuantity,
                db.TotalTime
            ))
            .ToList();
        list.Sort((a, b) => a.TotalTime.CompareTo(b.TotalTime));
        var data = new BrewByWeightHistoryEntry.ProcessFailed(
            p.Id,
            p.Timestamp,
            p.BeanId,
            p.InCupQuantity,
            p.GrindSetting,
            p.CoffeeQuantity,
            p.TargetExtractionTime,
            p.Spout,
            p.AverageFlow,
            p.TotalQuantity,
            list.ToArray(),
            p.TotalTime,
            p.ErrorType
        );
        return (p, runtimeData, data);
    }

    [Fact]
    public async Task ReadHistoryAsyncTest()
    {
        (
            ProcessDb DbProcess,
            ProcessRuntimeDataDb[] DbRuntime,
            BrewByWeightHistoryEntry Data
        )[] entries =
        [
            CreateFinishedHistoryEntry(
                Guid.NewGuid(),
                42,
                17,
                18,
                TimeSpan.FromSeconds(26),
                IBrewByWeightService.Spout.Single
            ),
            CreateFinishedHistoryEntry(
                Guid.NewGuid(),
                43,
                17,
                18,
                TimeSpan.FromSeconds(26),
                IBrewByWeightService.Spout.Single
            ),
            CreateFinishedHistoryEntry(
                Guid.NewGuid(),
                44,
                17,
                18,
                TimeSpan.FromSeconds(26),
                IBrewByWeightService.Spout.Single
            ),
            CreateFinishedHistoryEntry(
                Guid.NewGuid(),
                45,
                17,
                18,
                TimeSpan.FromSeconds(26),
                IBrewByWeightService.Spout.Single
            ),
            CreateCancelledHistoryEntry(
                Guid.NewGuid(),
                46,
                17,
                18,
                TimeSpan.FromSeconds(26),
                IBrewByWeightService.Spout.Single
            ),
            CreateFailedHistoryEntry(
                Guid.NewGuid(),
                47,
                17,
                18,
                TimeSpan.FromSeconds(26),
                IBrewByWeightService.Spout.Single,
                "Error"
            ),
        ];
        var processRepositoryMock = new Mock<IProcessRepository>();
        processRepositoryMock
            .Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>()))
            .Returns(
                Task.FromResult<IReadOnlyCollection<ProcessDb>>(
                    entries.Select(e => e.DbProcess).ToArray()
                )
            );
        var processRuntimeDataRepositoryMock = new Mock<IProcessRuntimeDataRepository>();
        processRuntimeDataRepositoryMock
            .Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>()))
            .Returns(
                Task.FromResult<IReadOnlyCollection<ProcessRuntimeDataDb>>(
                    entries.SelectMany(e => e.DbRuntime).ToArray()
                )
            );
        var service = new BrewByWeightHistoryService(
            processRepositoryMock.Object,
            processRuntimeDataRepositoryMock.Object,
            Mock.Of<ILogger<BrewByWeightHistoryService>>()
        );
        var history = (await service.ReadHistoryAsync(CancellationToken.None)).ToArray();
        Assert.Equal(6, history.Length);
        Assert.Equivalent(entries[0].Data, history[0]);
        Assert.Equivalent(entries[1].Data, history[1]);
        Assert.Equivalent(entries[2].Data, history[2]);
        processRepositoryMock.Verify(m => m.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        processRuntimeDataRepositoryMock.Verify(
            m => m.GetAllAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
        processRepositoryMock.VerifyNoOtherCalls();
        processRuntimeDataRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RemoveFromHistoryAsyncTest()
    {
        (
            ProcessDb DbProcess,
            ProcessRuntimeDataDb[] DbRuntime,
            BrewByWeightHistoryEntry Data
        )[] entries =
        [
            CreateFinishedHistoryEntry(
                Guid.NewGuid(),
                42,
                17,
                18,
                TimeSpan.FromSeconds(26),
                IBrewByWeightService.Spout.Single
            ),
            CreateFinishedHistoryEntry(
                Guid.NewGuid(),
                43,
                17,
                18,
                TimeSpan.FromSeconds(26),
                IBrewByWeightService.Spout.Single
            ),
        ];
        var processRepositoryMock = new Mock<IProcessRepository>();
        processRepositoryMock
            .Setup(m => m.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        processRepositoryMock
            .Setup(m => m.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var processRuntimeDataRepositoryMock = new Mock<IProcessRuntimeDataRepository>();
        processRuntimeDataRepositoryMock
            .Setup(m => m.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        processRuntimeDataRepositoryMock
            .Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>()))
            .Returns(
                Task.FromResult<IReadOnlyCollection<ProcessRuntimeDataDb>>(
                    entries.SelectMany(e => e.DbRuntime).ToArray()
                )
            );
        var service = new BrewByWeightHistoryService(
            processRepositoryMock.Object,
            processRuntimeDataRepositoryMock.Object,
            Mock.Of<ILogger<BrewByWeightHistoryService>>()
        );
        Assert.Equal(
            entries[0].DbProcess.Id,
            await service.RemoveFromHistoryAsync(entries[0].DbProcess.Id, CancellationToken.None)
        );
        processRepositoryMock.Verify(
            m => m.DeleteAsync(entries[0].DbProcess.Id, It.IsAny<CancellationToken>()),
            Times.Once
        );
        processRepositoryMock.Verify(m => m.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        processRepositoryMock.VerifyNoOtherCalls();
        processRuntimeDataRepositoryMock.Verify(
            m => m.GetAllAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
        foreach (var r in entries[0].DbRuntime)
            processRuntimeDataRepositoryMock.Verify(
                m => m.DeleteAsync(r.Id, It.IsAny<CancellationToken>()),
                Times.Once
            );
        processRuntimeDataRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CleanupHistoryAsyncTest()
    {
        var beanId = Guid.NewGuid();
        (
            ProcessDb DbProcess,
            ProcessRuntimeDataDb[] DbRuntime,
            BrewByWeightHistoryEntry Data
        )[] entries =
        [
            CreateFinishedHistoryEntry(
                beanId,
                42,
                17,
                18,
                TimeSpan.FromSeconds(26),
                IBrewByWeightService.Spout.Single
            ),
            CreateFinishedHistoryEntry(
                beanId,
                42,
                17,
                18,
                TimeSpan.FromSeconds(26),
                IBrewByWeightService.Spout.Single
            ),
            CreateFinishedHistoryEntry(
                beanId,
                43,
                17,
                18,
                TimeSpan.FromSeconds(26),
                IBrewByWeightService.Spout.Single
            ),
        ];
        var processRepositoryMock = new Mock<IProcessRepository>();
        processRepositoryMock
            .Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>()))
            .Returns(
                Task.FromResult<IReadOnlyCollection<ProcessDb>>(
                    entries.Select(e => e.DbProcess).ToArray()
                )
            );
        processRepositoryMock
            .Setup(m => m.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var processRuntimeDataRepositoryMock = new Mock<IProcessRuntimeDataRepository>();
        processRuntimeDataRepositoryMock
            .Setup(m => m.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        processRuntimeDataRepositoryMock
            .Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>()))
            .Returns(
                Task.FromResult<IReadOnlyCollection<ProcessRuntimeDataDb>>(
                    entries.SelectMany(e => e.DbRuntime).ToArray()
                )
            );
        var service = new BrewByWeightHistoryService(
            processRepositoryMock.Object,
            processRuntimeDataRepositoryMock.Object,
            Mock.Of<ILogger<BrewByWeightHistoryService>>()
        );
        var remaining = (await service.CleanupHistoryAsync(true, CancellationToken.None)).ToArray();
        Assert.Equal(2, remaining.Length);
        Assert.Contains(entries[1].Data.Id, remaining.Select(r => r.Id));
        Assert.Contains(entries[2].Data.Id, remaining.Select(r => r.Id));
        processRepositoryMock.Verify(m => m.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        processRuntimeDataRepositoryMock.Verify(
            m => m.GetAllAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
        processRepositoryMock.Verify(
            m => m.DeleteAsync(entries[0].DbProcess.Id, It.IsAny<CancellationToken>()),
            Times.Once
        );
        foreach (var r in entries[0].DbRuntime)
            processRuntimeDataRepositoryMock.Verify(
                m => m.DeleteAsync(r.Id, It.IsAny<CancellationToken>()),
                Times.Once
            );
        var nothing = await service.CleanupHistoryAsync(false, CancellationToken.None);
        Assert.Empty(nothing);
        processRepositoryMock.Verify(
            m => m.GetAllAsync(It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );
        processRuntimeDataRepositoryMock.Verify(
            m => m.GetAllAsync(It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );
        processRepositoryMock.Verify(
            m => m.DeleteAsync(entries[0].DbProcess.Id, It.IsAny<CancellationToken>()),
            Times.Exactly(2)
        );
        foreach (var r in entries[0].DbRuntime)
            processRuntimeDataRepositoryMock.Verify(
                m => m.DeleteAsync(r.Id, It.IsAny<CancellationToken>()),
                Times.Exactly(2)
            );
        processRepositoryMock.Verify(
            m => m.DeleteAsync(entries[1].DbProcess.Id, It.IsAny<CancellationToken>()),
            Times.Once
        );
        foreach (var r in entries[1].DbRuntime)
            processRuntimeDataRepositoryMock.Verify(
                m => m.DeleteAsync(r.Id, It.IsAny<CancellationToken>()),
                Times.Once
            );
        processRepositoryMock.Verify(
            m => m.DeleteAsync(entries[2].DbProcess.Id, It.IsAny<CancellationToken>()),
            Times.Once
        );
        foreach (var r in entries[2].DbRuntime)
            processRuntimeDataRepositoryMock.Verify(
                m => m.DeleteAsync(r.Id, It.IsAny<CancellationToken>()),
                Times.Once
            );
        processRepositoryMock.VerifyNoOtherCalls();
        processRuntimeDataRepositoryMock.VerifyNoOtherCalls();
    }
}
