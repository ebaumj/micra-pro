using MicraPro.ScaleManagement.DataDefinition;
using MicraPro.ScaleManagement.Domain.StorageAccess;

namespace MicraPro.ScaleManagement.Domain.ScaleImplementations;

public interface IScaleImplementationCollectionService
{
    IScale CreateScale(ScaleDb scaleDb);
    (string Name, string[] RequiredServices)[] Implementations { get; }
}
