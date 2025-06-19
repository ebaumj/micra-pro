namespace MicraPro.AssetManagement.Infrastructure.Interfaces;

public interface IHttpClientWrapperFactory
{
    public IHttpClientWrapper CreateClient(string bearerToken);
}
