using MicraPro.ScaleManagement.DataDefinition;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using MicraPro.ScaleManagement.Domain.StorageAccess;
using Microsoft.Extensions.DependencyInjection;

namespace MicraPro.ScaleManagement.Domain.ScaleImplementations;

public class ScaleImplementationCollectionService(IServiceProvider serviceProvider)
    : IScaleImplementationCollectionService
{
    public IScale CreateScale(ScaleDb scaleDb)
    {
        var service = serviceProvider
            .GetServices<IScaleImplementationFactory>()
            .FirstOrDefault(s => s.TypeName == scaleDb.ImplementationType);
        if (service is null)
            throw new Exception("Scale implementation not found!");
        return service.CreateScale(scaleDb.Identifier);
    }

    public (string Name, Func<BluetoothScanResult, bool> Filter)[] Implementations =>
        serviceProvider
            .GetServices<IScaleImplementationFactory>()
            .Select<
                IScaleImplementationFactory,
                (string Name, Func<BluetoothScanResult, bool> Filter)
            >(s => (s.TypeName, s.IsScaleType))
            .ToArray();
}
