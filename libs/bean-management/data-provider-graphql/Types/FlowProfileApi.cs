using MicraPro.BeanManagement.DataDefinition.ValueObjects;

namespace MicraPro.BeanManagement.DataProviderGraphQl.Types;

[GraphQLName("FlowProfile")]
public record FlowProfileApi(Guid Id, Guid RecipeId, FlowProfileProperties Properties);
