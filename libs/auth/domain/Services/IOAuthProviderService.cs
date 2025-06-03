using Microsoft.AspNetCore.Http;

namespace MicraPro.Auth.Domain.Services;

public interface IOAuthProviderService
{
    Task<string> HandleAuthRequestAsync(HttpRequest request);
}
