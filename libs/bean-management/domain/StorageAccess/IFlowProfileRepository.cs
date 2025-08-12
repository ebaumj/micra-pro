using MicraPro.BeanManagement.DataDefinition.ValueObjects;
using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.BeanManagement.Domain.StorageAccess;

public interface IFlowProfileRepository : IRepository<FlowProfileDb>
{
    Task<FlowProfileDb> UpdateAsync(
        Guid profileId,
        double startFlow,
        FlowSetting[] flowSettings,
        CancellationToken ct
    );
}
