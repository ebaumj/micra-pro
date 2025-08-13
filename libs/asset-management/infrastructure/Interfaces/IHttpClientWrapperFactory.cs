namespace MicraPro.AssetManagement.Infrastructure.Interfaces;

public interface IHttpClientWrapperFactory
{
    IHttpClientWrapper CreateClient(string bearerToken);
}
