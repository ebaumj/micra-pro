using MicraPro.BrewByWeight.DataDefinition;
using MicraPro.BrewByWeight.DataDefinition.ValueObjects;
using MicraPro.BrewByWeight.Domain.Services;
using MicraPro.BrewByWeight.Domain.StorageAccess;
using Moq;

namespace MicraPro.BrewByWeight.Domain.Test.Services;

public class BrewByWeightDbServiceTest
{
    public static readonly TheoryData<(
        BrewByWeightTracking LastTracking,
        Type expectedType
    )> ProcessResults = new()
    {
        (
            new BrewByWeightTracking.Finished(2, 41, TimeSpan.FromSeconds(25)),
            typeof(FinishedProcessDb)
        ),
        (
            new BrewByWeightTracking.Cancelled(2, 41, TimeSpan.FromSeconds(25)),
            typeof(CancelledProcessDb)
        ),
        (
            new BrewByWeightTracking.Failed(
                new BrewByWeightException.BrewServiceNotReady(),
                2,
                41,
                TimeSpan.FromSeconds(25)
            ),
            typeof(FailedProcessDb)
        ),
        (
            new BrewByWeightTracking.Failed(
                new BrewByWeightException.ScaleConnectionFailed(),
                2,
                41,
                TimeSpan.FromSeconds(25)
            ),
            typeof(FailedProcessDb)
        ),
        (
            new BrewByWeightTracking.Failed(
                new BrewByWeightException.UnknownError(),
                2,
                41,
                TimeSpan.FromSeconds(25)
            ),
            typeof(FailedProcessDb)
        ),
    };

    [Theory]
    [MemberData(nameof(ProcessResults))]
    public async Task StoreProcessAsyncTest(
        (BrewByWeightTracking lastTracking, Type expectedDbType) data
    )
    {
        var id = Guid.Empty;
        var processRepositoryMock = new Mock<IProcessRepository>();
        processRepositoryMock
            .Setup(m => m.AddAsync(It.IsAny<ProcessDb>(), It.IsAny<CancellationToken>()))
            .Callback(
                (ProcessDb p, CancellationToken _) =>
                {
                    id = p.Id;
                    Assert.Equal(42, p.InCupQuantity);
                    Assert.Equal(15, p.GrindSetting);
                    Assert.Equal(18, p.CoffeeQuantity);
                    Assert.Equal(TimeSpan.FromSeconds(26), p.TargetExtractionTime);
                    Assert.Equal(IBrewByWeightService.Spout.Single, p.Spout);
                    Assert.Equal(data.expectedDbType, p.GetType());
                    switch (data.lastTracking)
                    {
                        case BrewByWeightTracking.Finished:
                        {
                            var proc = (FinishedProcessDb)p;
                            Assert.Equal(2, proc.AverageFlow);
                            Assert.Equal(41, proc.TotalQuantity);
                            Assert.Equal(TimeSpan.FromSeconds(25), proc.ExtractionTime);
                            break;
                        }
                        case BrewByWeightTracking.Cancelled:
                        {
                            var proc = (CancelledProcessDb)p;
                            Assert.Equal(2, proc.AverageFlow);
                            Assert.Equal(41, proc.TotalQuantity);
                            Assert.Equal(TimeSpan.FromSeconds(25), proc.TotalTime);
                            break;
                        }
                        case BrewByWeightTracking.Failed failed:
                        {
                            var proc = (FailedProcessDb)p;
                            Assert.Equal(2, proc.AverageFlow);
                            Assert.Equal(41, proc.TotalQuantity);
                            Assert.Equal(TimeSpan.FromSeconds(25), proc.TotalTime);
                            Assert.Equal(failed.Exception.GetType().Name, proc.ErrorType);
                            break;
                        }
                    }
                }
            )
            .Returns(Task.CompletedTask);
        processRepositoryMock
            .Setup(m => m.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var processRuntimeDataRepository = new Mock<IProcessRuntimeDataRepository>();
        processRuntimeDataRepository
            .Setup(m =>
                m.AddRangeAsync(
                    It.IsAny<IReadOnlyCollection<ProcessRuntimeDataDb>>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Callback(
                (IReadOnlyCollection<ProcessRuntimeDataDb> p, CancellationToken _) =>
                {
                    var proc = p.ToArray();
                    Assert.Equal(2, proc.Length);
                    Assert.Equal(id, proc[0].ProcessId);
                    Assert.Equal(10, proc[0].Flow);
                    Assert.Equal(2, proc[0].TotalQuantity);
                    Assert.Equal(TimeSpan.FromSeconds(1), proc[0].TotalTime);
                    Assert.Equal(id, proc[1].ProcessId);
                    Assert.Equal(5, proc[1].Flow);
                    Assert.Equal(12, proc[1].TotalQuantity);
                    Assert.Equal(TimeSpan.FromSeconds(2), proc[1].TotalTime);
                }
            )
            .Returns(Task.CompletedTask);
        processRuntimeDataRepository
            .Setup(m => m.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var service = new BrewByWeightDbService(
            processRepositoryMock.Object,
            processRuntimeDataRepository.Object
        );
        BrewByWeightTracking.Running[] trackingUpdate =
        [
            new(10, 2, TimeSpan.FromSeconds(1)),
            new(5, 12, TimeSpan.FromSeconds(2)),
        ];
        await service.StoreProcessAsync(
            Guid.NewGuid(),
            Guid.NewGuid(),
            42,
            15,
            18,
            TimeSpan.FromSeconds(26),
            IBrewByWeightService.Spout.Single,
            [.. trackingUpdate, data.lastTracking],
            CancellationToken.None
        );
        processRepositoryMock.Verify(
            m => m.AddAsync(It.IsAny<ProcessDb>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        processRepositoryMock.Verify(m => m.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        processRuntimeDataRepository.Verify(
            m =>
                m.AddRangeAsync(
                    It.IsAny<IReadOnlyCollection<ProcessRuntimeDataDb>>(),
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
        processRuntimeDataRepository.Verify(
            m => m.SaveAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
        processRepositoryMock.VerifyNoOtherCalls();
        processRuntimeDataRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetFinishedAsyncTest()
    {
        var processRepositoryMock = new Mock<IProcessRepository>();
        processRepositoryMock
            .Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>()))
            .Returns(() =>
                Task.FromResult<IReadOnlyCollection<ProcessDb>>(
                    [
                        new FailedProcessDb(
                            Guid.NewGuid(),
                            Guid.NewGuid(),
                            0,
                            0,
                            0,
                            TimeSpan.Zero,
                            IBrewByWeightService.Spout.Naked,
                            0,
                            0,
                            TimeSpan.Zero,
                            ""
                        ),
                        new CancelledProcessDb(
                            Guid.NewGuid(),
                            Guid.NewGuid(),
                            0,
                            0,
                            0,
                            TimeSpan.Zero,
                            IBrewByWeightService.Spout.Naked,
                            0,
                            0,
                            TimeSpan.Zero
                        ),
                        new FinishedProcessDb(
                            Guid.NewGuid(),
                            Guid.NewGuid(),
                            0,
                            0,
                            0,
                            TimeSpan.Zero,
                            IBrewByWeightService.Spout.Naked,
                            0,
                            0,
                            TimeSpan.Zero
                        ),
                    ]
                )
            );
        var service = new BrewByWeightDbService(
            processRepositoryMock.Object,
            Mock.Of<IProcessRuntimeDataRepository>()
        );
        Assert.Single(await service.GetFinishedAsync(CancellationToken.None));
        Assert.IsType<FinishedProcessDb>(
            (await service.GetFinishedAsync(CancellationToken.None)).FirstOrDefault()
        );
    }
}
