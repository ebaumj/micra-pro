namespace MicraPro.AssetManagement.Infrastructure.Interfaces;

public interface IHttpClientWrapper : IDisposable
{
    Task<string> MakeGetRequestAsync(string url, CancellationToken ct);
    Task MakeDeleteRequestAsync(string url, CancellationToken ct);
    Task MakePostRequest(string url, object body, CancellationToken ct);
}
