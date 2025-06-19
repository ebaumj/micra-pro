using MicraPro.AssetManagement.Infrastructure.Interfaces;

namespace MicraPro.AssetManagement.Infrastructure.Services;

public class HttpClientWrapperFactory : IHttpClientWrapperFactory
{
    private class HttpClientWrapper(HttpClient client) : IHttpClientWrapper
    {
        public void Dispose()
        {
            client.Dispose();
        }

        public async Task<string> MakeGetRequestAsync(string url, CancellationToken ct) =>
            await (await client.GetAsync(url, ct)).Content.ReadAsStringAsync(ct);

        public Task MakeDeleteRequestAsync(string url, CancellationToken ct) =>
            client.DeleteAsync(url, ct);
    }

    public IHttpClientWrapper CreateClient(string bearerToken)
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
        return new HttpClientWrapper(client);
    }
}
