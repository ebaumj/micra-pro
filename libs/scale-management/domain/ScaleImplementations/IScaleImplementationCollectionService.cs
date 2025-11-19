using MicraPro.ScaleManagement.DataDefinition;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using MicraPro.ScaleManagement.Domain.StorageAccess;

namespace MicraPro.ScaleManagement.Domain.ScaleImplementations;

public interface IScaleImplementationCollectionService
{
    IScale CreateScale(ScaleDb scaleDb);
    (string Name, Func<BluetoothScanResult, bool> Filter)[] Implementations { get; }
}
