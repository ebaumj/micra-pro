using MicraPro.BeanManagement.DataDefinition.ValueObjects;
using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.BeanManagement.Domain.StorageAccess;

public class FlowProfileDb : IEntity
{
    public Guid Id { get; }
    public Guid RecipeId { get; }
    public double StartFlow { get; set; }
    public FlowSetting[] FlowSettings { get; set; }

    public RecipeDb RecipeObject { get; init; } = null!;

    private FlowProfileDb(Guid id, Guid recipeId, double startFlow, FlowSetting[] flowSettings)
    {
        Id = id;
        RecipeId = recipeId;
        StartFlow = startFlow;
        FlowSettings = flowSettings;
    }

    public FlowProfileDb(Guid recipeId, double startFlow, FlowSetting[] flowSettings)
        : this(Guid.NewGuid(), recipeId, startFlow, flowSettings) { }
}
