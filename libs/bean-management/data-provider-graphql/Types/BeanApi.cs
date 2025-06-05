using MicraPro.BeanManagement.DataDefinition.ValueObjects;

namespace MicraPro.BeanManagement.DataProviderGraphQl.Types;

[GraphQLName("Bean")]
public record BeanApi(Guid Id, Guid RoasteryId, BeanProperties Properties);
