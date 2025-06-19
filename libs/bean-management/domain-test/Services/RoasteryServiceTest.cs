using MicraPro.BeanManagement.DataDefinition.ValueObjects;
using MicraPro.BeanManagement.Domain.Services;
using MicraPro.BeanManagement.Domain.StorageAccess;
using Moq;

namespace MicraPro.BeanManagement.Domain.Test.Services;

public class RoasteryServiceTest
{
    [Fact]
    public async Task AddRoasteryAsyncTest()
    {
        var properties = new RoasteryProperties("SomeName", "SomeLocation");
        var repositoryMock = new Mock<IRoasteryRepository>();
        repositoryMock
            .Setup(m => m.AddAsync(It.IsAny<RoasteryDb>(), It.IsAny<CancellationToken>()))
            .Callback(
                (RoasteryDb r, CancellationToken _) =>
                {
                    Assert.Equal("SomeName", r.Name);
                    Assert.Equal("SomeLocation", r.Location);
                }
            )
            .Returns(Task.CompletedTask);
        repositoryMock
            .Setup(m => m.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var result = await new RoasteryService(repositoryMock.Object).AddRoasteryAsync(
            properties,
            CancellationToken.None
        );
        Assert.Equal(properties, result.Properties);
        repositoryMock.Verify(
            m => m.AddAsync(It.IsAny<RoasteryDb>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        repositoryMock.Verify(m => m.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetRoasteriesAsyncTest()
    {
        var roastery1 = new RoasteryDb("Name1", "Location1");
        var roastery2 = new RoasteryDb("Name2", "Location2");
        var repositoryMock = new Mock<IRoasteryRepository>();
        repositoryMock
            .Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult<IReadOnlyCollection<RoasteryDb>>([roastery1, roastery2])
            );
        var result = (
            await new RoasteryService(repositoryMock.Object).GetRoasteriesAsync(
                CancellationToken.None
            )
        ).ToArray();
        var result1 = result.FirstOrDefault(b => b.Id == roastery1.Id);
        var result2 = result.FirstOrDefault(b => b.Id == roastery2.Id);
        Assert.NotNull(result1);
        Assert.Equal("Name1", result1.Properties.Name);
        Assert.Equal("Location1", result1.Properties.Location);
        Assert.NotNull(result2);
        Assert.Equal("Name2", result2.Properties.Name);
        Assert.Equal("Location2", result2.Properties.Location);
        repositoryMock.Verify(m => m.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateRoasteryAsyncTest()
    {
        var newRoastery = new RoasteryDb("NewName", "NewLocation");
        var repositoryMock = new Mock<IRoasteryRepository>();
        repositoryMock
            .Setup(m =>
                m.UpdateAsync(
                    newRoastery.Id,
                    newRoastery.Name,
                    newRoastery.Location,
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.FromResult(newRoastery));
        var result = await new RoasteryService(repositoryMock.Object).UpdateRoasteryAsync(
            newRoastery.Id,
            new RoasteryProperties("NewName", "NewLocation"),
            CancellationToken.None
        );
        Assert.Equal("NewName", result.Properties.Name);
        Assert.Equal("NewLocation", result.Properties.Location);
        Assert.Equal(newRoastery.Id, result.Id);
        repositoryMock.Verify(
            m =>
                m.UpdateAsync(
                    newRoastery.Id,
                    newRoastery.Name,
                    newRoastery.Location,
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
        repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RemoveRoasteryAsyncTest()
    {
        var id = Guid.NewGuid();
        var repositoryMock = new Mock<IRoasteryRepository>();
        repositoryMock
            .Setup(m => m.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        repositoryMock
            .Setup(m => m.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var result = await new RoasteryService(repositoryMock.Object).RemoveRoasteryAsync(
            id,
            CancellationToken.None
        );
        Assert.Equal(id, result);
        repositoryMock.Verify(m => m.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(m => m.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
    }
}
