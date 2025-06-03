namespace MicraPro.BeanManagement.DataProviderGraphQl.Types;

[GraphQLName("Recipe")]
public record RecipeApi(Guid Id, Guid BeanId, RecipePropertiesApi Properties);
