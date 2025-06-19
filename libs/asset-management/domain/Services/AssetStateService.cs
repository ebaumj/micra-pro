using System.Reactive.Subjects;
using MicraPro.AssetManagement.DataDefinition;

namespace MicraPro.AssetManagement.Domain.Services;

public class AssetStateService
{
    private readonly BehaviorSubject<IEnumerable<IAsset>> _assets = new([]);
    public ISubject<IEnumerable<IAsset>> Assets => _assets;
}
