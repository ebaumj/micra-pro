using MicraPro.AssetManagement.DataDefinition;

namespace MicraPro.AssetManagement.Domain.ValueObjects;

public record Asset(Guid Id, string Path, bool IsAvailableLocally, bool IsAvailableRemotely)
    : IAsset;
