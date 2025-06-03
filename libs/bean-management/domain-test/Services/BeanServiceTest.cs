using MicraPro.BeanManagement.DataDefinition.ValueObjects;
using MicraPro.BeanManagement.Domain.Services;
using MicraPro.BeanManagement.Domain.StorageAccess;
using MicraPro.BeanManagement.Domain.ValueObjects;
using Moq;

namespace MicraPro.BeanManagement.Domain.Test.Services;

public class BeanServiceTest
{
    [Fact]
    public async Task AddBeanAsyncTest()
    {
        var properties = new BeanProperties("Some Name", "Code", null);
        var roasteryId = Guid.NewGuid();
        var repositoryMock = new Mock<IBeanRepository>();
        repositoryMock
            .Setup(m => m.AddAsync(It.IsAny<BeanDb>(), It.IsAny<CancellationToken>()))
            .Callback(
                (BeanDb b, CancellationToken _) =>
                {
                    Assert.Equal("Some Name", b.Name);
                    Assert.Equal("Code", b.CountryCode);
                    Assert.Equal(Guid.Empty, b.AssetId);
                    Assert.Equal(roasteryId, b.RoasteryId);
                }
            )
            .Returns(Task.CompletedTask);
        repositoryMock
            .Setup(m => m.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var result = await new BeanService(repositoryMock.Object).AddBeanAsync(
            properties,
            roasteryId,
            CancellationToken.None
        );
        Assert.Equal(properties, result.Properties);
        Assert.Equal(roasteryId, result.RoasteryId);
        repositoryMock.Verify(
            m => m.AddAsync(It.IsAny<BeanDb>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        repositoryMock.Verify(m => m.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetBeansAsyncTest()
    {
        var roasteryId = Guid.NewGuid();
        var assetId = Guid.NewGuid();
        var bean1 = new BeanDb("Name1", roasteryId, "Code1", Guid.Empty);
        var bean2 = new BeanDb("Name2", roasteryId, "Code2", assetId);
        var repositoryMock = new Mock<IBeanRepository>();
        repositoryMock
            .Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult<IReadOnlyCollection<BeanDb>>([bean1, bean2]));
        var result = (
            await new BeanService(repositoryMock.Object).GetBeansAsync(CancellationToken.None)
        ).ToArray();
        var result1 = result.FirstOrDefault(b => b.Id == bean1.Id);
        var result2 = result.FirstOrDefault(b => b.Id == bean2.Id);
        Assert.NotNull(result1);
        Assert.Equal("Name1", result1.Properties.Name);
        Assert.Equal("Code1", result1.Properties.CountryCode);
        Assert.Null(result1.Properties.AssetId);
        Assert.Equal(roasteryId, result1.RoasteryId);
        Assert.NotNull(result2);
        Assert.Equal("Name2", result2.Properties.Name);
        Assert.Equal("Code2", result2.Properties.CountryCode);
        Assert.Equal(assetId, result2.Properties.AssetId);
        Assert.Equal(roasteryId, result2.RoasteryId);
        repositoryMock.Verify(m => m.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateBeanAsyncTest()
    {
        var newBean = new BeanDb("NewName", Guid.NewGuid(), "NewCode", Guid.Empty);
        var repositoryMock = new Mock<IBeanRepository>();
        repositoryMock
            .Setup(m =>
                m.UpdateAsync(
                    newBean.Id,
                    newBean.Name,
                    newBean.CountryCode,
                    newBean.AssetId,
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.FromResult(newBean));
        var result = await new BeanService(repositoryMock.Object).UpdateBeanAsync(
            newBean.Id,
            new BeanProperties("NewName", "NewCode", null),
            CancellationToken.None
        );
        Assert.Equal("NewName", result.Properties.Name);
        Assert.Equal("NewCode", result.Properties.CountryCode);
        Assert.Equal(newBean.Id, result.Id);
        Assert.Equal(newBean.RoasteryId, result.RoasteryId);
        Assert.Null(result.Properties.AssetId);
        repositoryMock.Verify(
            m =>
                m.UpdateAsync(
                    newBean.Id,
                    newBean.Name,
                    newBean.CountryCode,
                    newBean.AssetId,
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
        repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RemoveBeanAsyncTest()
    {
        var id = Guid.NewGuid();
        var repositoryMock = new Mock<IBeanRepository>();
        repositoryMock
            .Setup(m => m.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        repositoryMock
            .Setup(m => m.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var result = await new BeanService(repositoryMock.Object).RemoveBeanAsync(
            id,
            CancellationToken.None
        );
        Assert.Equal(id, result);
        repositoryMock.Verify(m => m.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(m => m.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
    }
}
