using MicraPro.ScaleManagement.DataDefinition;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;

namespace MicraPro.ScaleManagement.Domain.ScaleImplementations;

public interface IScaleImplementationFactory
{
    string TypeName { get; }
    bool IsScaleType(BluetoothScanResult result);
    IScale CreateScale(string identifier);
}
