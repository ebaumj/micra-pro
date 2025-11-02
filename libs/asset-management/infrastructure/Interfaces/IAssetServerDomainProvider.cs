namespace MicraPro.AssetManagement.Infrastructure.Interfaces;

public interface IAssetServerDomainProvider
{
    string AssetServerLocalDomain { get; }
    string AssetServerExternDomain { get; }
}
