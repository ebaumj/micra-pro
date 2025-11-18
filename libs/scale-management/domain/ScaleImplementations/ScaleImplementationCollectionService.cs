using System.Collections.Immutable;
using MicraPro.ScaleManagement.DataDefinition;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using MicraPro.ScaleManagement.Domain.StorageAccess;

namespace MicraPro.ScaleManagement.Domain.ScaleImplementations;

public class ScaleImplementationCollectionService(IBluetoothService bluetoothService)
    : IScaleImplementationCollectionService
{
    private record ScaleImplementation(
        string Name,
        Func<ScaleDb, IScale> CreateScale,
        string[] RequiredServices
    );

    private readonly IImmutableList<ScaleImplementation> _scaleImplementations =
    [
        new(
            typeof(BookooThemisMini.Scale).FullName!,
            s => new BookooThemisMini.Scale(s.Identifier, bluetoothService),
            BookooThemisMini.Scale.RequiredServiceIds
        ),
    ];

    public IScale CreateScale(ScaleDb scaleDb) =>
        _scaleImplementations
            .FirstOrDefault(i => i.Name == scaleDb.ImplementationType)
            ?.CreateScale(scaleDb) ?? throw new Exception("Scale implementation not found!");

    public (string Name, string[] RequiredServices)[] Implementations =>
        _scaleImplementations.Select(i => (i.Name, i.RequiredServices)).ToArray();
}
