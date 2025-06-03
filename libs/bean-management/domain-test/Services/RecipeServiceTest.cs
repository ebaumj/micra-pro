using MicraPro.BeanManagement.DataDefinition.ValueObjects;
using MicraPro.BeanManagement.Domain.Services;
using MicraPro.BeanManagement.Domain.StorageAccess;
using Moq;

namespace MicraPro.BeanManagement.Domain.Test.Services;

public class RecipeServiceTest
{
    [Fact]
    public async Task AddEspressoRecipeAsyncTest()
    {
        var properties = new RecipeProperties.Espresso(15, 18, 42, 93, TimeSpan.FromSeconds(26));
        var beanId = Guid.NewGuid();
        var repositoryMock = new Mock<IRecipeRepository>();
        repositoryMock
            .Setup(m => m.AddAsync(It.IsAny<EspressoRecipeDb>(), It.IsAny<CancellationToken>()))
            .Callback(
                (RecipeDb rec, CancellationToken _) =>
                {
                    Assert.IsType<EspressoRecipeDb>(rec);
                    var r = (EspressoRecipeDb)rec;
                    Assert.Equal(beanId, r.BeanId);
                    Assert.Equal(15, r.GrindSetting);
                    Assert.Equal(18, r.CoffeeQuantity);
                    Assert.Equal(42, r.InCupQuantity);
                    Assert.Equal(93, r.BrewTemperature);
                    Assert.Equal(TimeSpan.FromSeconds(26), r.TargetExtractionTime);
                }
            )
            .Returns(Task.CompletedTask);
        repositoryMock
            .Setup(m => m.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var result = await new RecipeService(repositoryMock.Object).AddRecipeAsync(
            properties,
            beanId,
            CancellationToken.None
        );
        Assert.Equal(properties, result.Properties);
        Assert.Equal(beanId, result.BeanId);
        repositoryMock.Verify(
            m => m.AddAsync(It.IsAny<EspressoRecipeDb>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        repositoryMock.Verify(m => m.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task AddV60RecipeAsyncTest()
    {
        var properties = new RecipeProperties.V60(78, 25, 360, 92);
        var beanId = Guid.NewGuid();
        var repositoryMock = new Mock<IRecipeRepository>();
        repositoryMock
            .Setup(m => m.AddAsync(It.IsAny<V60RecipeDb>(), It.IsAny<CancellationToken>()))
            .Callback(
                (RecipeDb rec, CancellationToken _) =>
                {
                    Assert.IsType<V60RecipeDb>(rec);
                    var r = (V60RecipeDb)rec;
                    Assert.Equal(beanId, r.BeanId);
                    Assert.Equal(78, r.GrindSetting);
                    Assert.Equal(25, r.CoffeeQuantity);
                    Assert.Equal(360, r.InCupQuantity);
                    Assert.Equal(92, r.BrewTemperature);
                }
            )
            .Returns(Task.CompletedTask);
        repositoryMock
            .Setup(m => m.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var result = await new RecipeService(repositoryMock.Object).AddRecipeAsync(
            properties,
            beanId,
            CancellationToken.None
        );
        Assert.Equal(properties, result.Properties);
        Assert.Equal(beanId, result.BeanId);
        repositoryMock.Verify(
            m => m.AddAsync(It.IsAny<V60RecipeDb>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        repositoryMock.Verify(m => m.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetRecipesAsyncTest()
    {
        var beanId = Guid.NewGuid();
        var recipe1 = new EspressoRecipeDb(beanId, 15, 18, 42, 93, TimeSpan.FromSeconds(26));
        var recipe2 = new V60RecipeDb(beanId, 78, 25, 360, 92);
        var repositoryMock = new Mock<IRecipeRepository>();
        repositoryMock
            .Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult<IReadOnlyCollection<RecipeDb>>([recipe1, recipe2]));
        var result = (
            await new RecipeService(repositoryMock.Object).GetRecipesAsync(CancellationToken.None)
        ).ToArray();
        var result1 = result.FirstOrDefault(b => b.Id == recipe1.Id);
        var result2 = result.FirstOrDefault(b => b.Id == recipe2.Id);
        Assert.NotNull(result1);
        Assert.IsType<RecipeProperties.Espresso>(result1.Properties);
        var props1 = (RecipeProperties.Espresso)result1.Properties;
        Assert.Equal(15, props1.GrindSetting);
        Assert.Equal(18, props1.CoffeeQuantity);
        Assert.Equal(42, props1.InCupQuantity);
        Assert.Equal(93, props1.BrewTemperature);
        Assert.Equal(TimeSpan.FromSeconds(26), props1.TargetExtractionTime);
        Assert.Equal(beanId, result1.BeanId);
        Assert.NotNull(result2);
        Assert.IsType<RecipeProperties.V60>(result2.Properties);
        var props2 = (RecipeProperties.V60)result2.Properties;
        Assert.Equal(78, props2.GrindSetting);
        Assert.Equal(25, props2.CoffeeQuantity);
        Assert.Equal(360, props2.InCupQuantity);
        Assert.Equal(92, props2.BrewTemperature);
        Assert.Equal(beanId, result2.BeanId);
        repositoryMock.Verify(m => m.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateEspressoRecipeAsyncTest()
    {
        var newRecipe = new EspressoRecipeDb(
            Guid.NewGuid(),
            15,
            18,
            42,
            93,
            TimeSpan.FromSeconds(26)
        );
        var repositoryMock = new Mock<IRecipeRepository>();
        repositoryMock
            .Setup(m =>
                m.UpdateEspressoAsync(
                    newRecipe.Id,
                    newRecipe.GrindSetting,
                    newRecipe.CoffeeQuantity,
                    newRecipe.InCupQuantity,
                    newRecipe.BrewTemperature,
                    newRecipe.TargetExtractionTime,
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.FromResult(newRecipe));
        var result = await new RecipeService(repositoryMock.Object).UpdateRecipeAsync(
            newRecipe.Id,
            new RecipeProperties.Espresso(15, 18, 42, 93, TimeSpan.FromSeconds(26)),
            CancellationToken.None
        );
        Assert.IsType<RecipeProperties.Espresso>(result.Properties);
        var props1 = (RecipeProperties.Espresso)result.Properties;
        Assert.Equal(15, props1.GrindSetting);
        Assert.Equal(18, props1.CoffeeQuantity);
        Assert.Equal(42, props1.InCupQuantity);
        Assert.Equal(93, props1.BrewTemperature);
        Assert.Equal(TimeSpan.FromSeconds(26), props1.TargetExtractionTime);
        Assert.Equal(newRecipe.BeanId, result.BeanId);
        Assert.Equal(newRecipe.Id, result.Id);
        repositoryMock.Verify(
            m =>
                m.UpdateEspressoAsync(
                    newRecipe.Id,
                    newRecipe.GrindSetting,
                    newRecipe.CoffeeQuantity,
                    newRecipe.InCupQuantity,
                    newRecipe.BrewTemperature,
                    newRecipe.TargetExtractionTime,
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
        repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateV60RecipeAsyncTest()
    {
        var newRecipe = new V60RecipeDb(Guid.NewGuid(), 78, 25, 360, 92);
        var repositoryMock = new Mock<IRecipeRepository>();
        repositoryMock
            .Setup(m =>
                m.UpdateV60Async(
                    newRecipe.Id,
                    newRecipe.GrindSetting,
                    newRecipe.CoffeeQuantity,
                    newRecipe.InCupQuantity,
                    newRecipe.BrewTemperature,
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.FromResult(newRecipe));
        var result = await new RecipeService(repositoryMock.Object).UpdateRecipeAsync(
            newRecipe.Id,
            new RecipeProperties.V60(78, 25, 360, 92),
            CancellationToken.None
        );
        Assert.IsType<RecipeProperties.V60>(result.Properties);
        var props1 = (RecipeProperties.V60)result.Properties;
        Assert.Equal(78, props1.GrindSetting);
        Assert.Equal(25, props1.CoffeeQuantity);
        Assert.Equal(360, props1.InCupQuantity);
        Assert.Equal(92, props1.BrewTemperature);
        Assert.Equal(newRecipe.BeanId, result.BeanId);
        Assert.Equal(newRecipe.Id, result.Id);
        repositoryMock.Verify(
            m =>
                m.UpdateV60Async(
                    newRecipe.Id,
                    newRecipe.GrindSetting,
                    newRecipe.CoffeeQuantity,
                    newRecipe.InCupQuantity,
                    newRecipe.BrewTemperature,
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
        repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RemoveRecipeAsyncTest()
    {
        var id = Guid.NewGuid();
        var repositoryMock = new Mock<IRecipeRepository>();
        repositoryMock
            .Setup(m => m.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        repositoryMock
            .Setup(m => m.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var result = await new RecipeService(repositoryMock.Object).RemoveRecipeAsync(
            id,
            CancellationToken.None
        );
        Assert.Equal(id, result);
        repositoryMock.Verify(m => m.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.Verify(m => m.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        repositoryMock.VerifyNoOtherCalls();
    }
}
