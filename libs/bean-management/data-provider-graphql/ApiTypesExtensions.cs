using MicraPro.BeanManagement.DataDefinition;
using MicraPro.BeanManagement.DataDefinition.ValueObjects;
using MicraPro.BeanManagement.DataProviderGraphQl.Types;

namespace MicraPro.BeanManagement.DataProviderGraphQl;

public static class ApiTypesExtensions
{
    public static BeanApi ToApi(this IBean bean) => new(bean.Id, bean.RoasteryId, bean.Properties);

    public static RoasteryApi ToApi(this IRoastery roastery) =>
        new(roastery.Id, roastery.Properties);

    public static RecipeApi ToApi(this IRecipe recipe) =>
        recipe.Properties switch
        {
            RecipeProperties.Espresso r => new RecipeApi(
                recipe.Id,
                recipe.BeanId,
                new RecipePropertiesApi.EspressoApi(r)
            ),
            RecipeProperties.V60 r => new RecipeApi(
                recipe.Id,
                recipe.BeanId,
                new RecipePropertiesApi.V60Api(r)
            ),
            _ => throw new NotImplementedException(
                $"Recipe Properties Type {recipe.Properties.GetType().Name} is not implemented!"
            ),
        };
}
