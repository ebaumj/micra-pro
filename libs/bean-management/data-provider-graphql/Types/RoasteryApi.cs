using MicraPro.BeanManagement.DataDefinition.ValueObjects;

namespace MicraPro.BeanManagement.DataProviderGraphQl.Types;

[GraphQLName("Roastery")]
public record RoasteryApi(Guid Id, RoasteryProperties Properties);
