using MicraPro.BeanManagement.DataDefinition.ValueObjects;

namespace MicraPro.BeanManagement.DataProviderGraphQl.Types;

[UnionType("RecipeProperties")]
public abstract record RecipePropertiesApi
{
    [ObjectType("EspressoProperties")]
    public record EspressoApi(RecipeProperties.Espresso Properties) : RecipePropertiesApi;

    [ObjectType("V60Properties")]
    public record V60Api(RecipeProperties.V60 Properties) : RecipePropertiesApi;
}
