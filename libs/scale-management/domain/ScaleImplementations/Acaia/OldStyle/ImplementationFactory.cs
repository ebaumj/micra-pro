using MicraPro.ScaleManagement.DataDefinition;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;

namespace MicraPro.ScaleManagement.Domain.ScaleImplementations.Acaia.OldStyle;

[ScaleImplementationFactory]
public class ImplementationFactory(IBluetoothService bluetoothService) : IScaleImplementationFactory
{
    public string TypeName => typeof(Scale).FullName!;

    public bool IsScaleType(BluetoothScanResult result) =>
        Scale
            .RequiredServiceIds.Select(s => s.ToLower())
            .All(s => result.ServiceIds.Select(id => id.ToLower()).Contains(s));

    public IScale CreateScale(string identifier) => new Scale(identifier, bluetoothService);
}
