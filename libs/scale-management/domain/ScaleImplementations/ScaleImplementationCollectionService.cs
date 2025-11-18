using System.Collections.Immutable;
using MicraPro.ScaleManagement.DataDefinition;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using MicraPro.ScaleManagement.Domain.ScaleImplementations.AcaiaLunar;
using MicraPro.ScaleManagement.Domain.StorageAccess;

namespace MicraPro.ScaleManagement.Domain.ScaleImplementations;

public class ScaleImplementationCollectionService(IBluetoothService bluetoothService)
    : IScaleImplementationCollectionService
{
    private record ScaleImplementation(
        string Name,
        Func<ScaleDb, IScale> CreateScale,
        Func<BluetoothScanResult, bool> Filter
    );

    private readonly IImmutableList<ScaleImplementation> _scaleImplementations =
    [
        new(
            typeof(BookooThemisMini.Scale).FullName!,
            s => new BookooThemisMini.Scale(s.Identifier, bluetoothService),
            dev =>
                BookooThemisMini
                    .Scale.RequiredServiceIds.Select(s => s.ToLower())
                    .All(s => dev.ServiceIds.Select(id => id.ToLower()).Contains(s))
        ),
        new(
            typeof(Scale).FullName!,
            s => new Scale(s.Identifier, bluetoothService),
            dev =>
                Scale
                    .RequiredServiceIds.Select(s => s.ToLower())
                    .All(s => dev.ServiceIds.Select(id => id.ToLower()).Contains(s))
        ),
    ];

    public IScale CreateScale(ScaleDb scaleDb) =>
        _scaleImplementations
            .FirstOrDefault(i => i.Name == scaleDb.ImplementationType)
            ?.CreateScale(scaleDb) ?? throw new Exception("Scale implementation not found!");

    public (string Name, Func<BluetoothScanResult, bool> Filter)[] Implementations =>
        _scaleImplementations.Select(i => (i.Name, i.Filter)).ToArray();
}
