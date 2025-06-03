using MicraPro.ScaleManagement.DataDefinition;

namespace MicraPro.ScaleManagement.DataProviderGraphQl.Types;

[GraphQLName("Scale")]
public record ScaleApi(Guid Id, string Name)
{
    public ScaleApi(IScale scale)
        : this(scale.Id, scale.Name) { }
}
