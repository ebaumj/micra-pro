using MicraPro.BeanManagement.DataDefinition.ValueObjects;
using MicraPro.BeanManagement.Domain.StorageAccess;
using Moq;

namespace MicraPro.BeanManagement.Domain.Test;

public class RecipeExtensionsCoverageTest
{
    public static readonly TheoryData<Type> RecipePropertiesTypes = new(
        typeof(RecipeProperties)
            .Assembly.GetTypes()
            .Where(t => typeof(RecipeProperties).IsAssignableFrom(t) && !t.IsAbstract)
    );
    public static readonly TheoryData<Type> RecipeDbTypes = new(
        typeof(RecipeDb)
            .Assembly.GetTypes()
            .Where(t => typeof(RecipeDb).IsAssignableFrom(t) && !t.IsAbstract)
    );

    private static T? CreateObject<T>(Type type) =>
        (T?)
            Activator.CreateInstance(
                type,
                type.GetConstructors()
                    .Single(c => c.IsPublic)
                    .GetParameters()
                    .Select(p => p.ParameterType)
                    .Select(t => t.IsValueType ? Activator.CreateInstance(t) : null)
                    .Cast<object>()
                    .ToArray()
            );

    [Theory]
    [MemberData(nameof(RecipeDbTypes))]
    public void ToRecipeTest(Type recipeDbType)
    {
        var instance = CreateObject<RecipeDb>(recipeDbType);
        Assert.NotNull(instance);
        Assert.NotNull(instance.ToRecipe());
    }

    [Theory]
    [MemberData(nameof(RecipePropertiesTypes))]
    public void ToRecipeDbTest(Type recipePropertiesType)
    {
        var instance = CreateObject<RecipeProperties>(recipePropertiesType);
        Assert.NotNull(instance);
        Assert.NotNull(instance.ToRecipeDb(Guid.Empty));
    }

    [Theory]
    [MemberData(nameof(RecipePropertiesTypes))]
    public async Task UpdateTest(Type recipePropertiesType)
    {
        var recipeRepositoryMock = new Mock<IRecipeRepository>();
        recipeRepositoryMock
            .Setup(m =>
                m.UpdateEspressoAsync(
                    It.IsAny<Guid>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<TimeSpan>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.FromResult(CreateObject<EspressoRecipeDb>(typeof(EspressoRecipeDb))!));
        recipeRepositoryMock
            .Setup(m =>
                m.UpdateV60Async(
                    It.IsAny<Guid>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.FromResult(CreateObject<V60RecipeDb>(typeof(V60RecipeDb))!));
        var instance = CreateObject<RecipeProperties>(recipePropertiesType);
        await recipeRepositoryMock.Object.Update(Guid.Empty, instance!, CancellationToken.None);
        switch (instance)
        {
            case RecipeProperties.Espresso:
                recipeRepositoryMock.Verify(m =>
                    m.UpdateEspressoAsync(
                        It.IsAny<Guid>(),
                        It.IsAny<double>(),
                        It.IsAny<double>(),
                        It.IsAny<double>(),
                        It.IsAny<double>(),
                        It.IsAny<TimeSpan>(),
                        It.IsAny<CancellationToken>()
                    )
                );
                break;
            case RecipeProperties.V60:
                recipeRepositoryMock.Verify(m =>
                    m.UpdateV60Async(
                        It.IsAny<Guid>(),
                        It.IsAny<double>(),
                        It.IsAny<double>(),
                        It.IsAny<double>(),
                        It.IsAny<double>(),
                        It.IsAny<CancellationToken>()
                    )
                );
                break;
            default:
                Assert.Fail("Type not implemented!");
                break;
        }
        recipeRepositoryMock.VerifyNoOtherCalls();
    }

    [Theory]
    [MemberData(nameof(RecipePropertiesTypes))]
    public void WithGrinderOffsetTest(Type recipePropertiesType)
    {
        var instance = CreateObject<RecipeProperties>(recipePropertiesType);
        Assert.NotNull(instance);
        Assert.NotNull(instance.WithGrinderOffset(0));
    }
}
