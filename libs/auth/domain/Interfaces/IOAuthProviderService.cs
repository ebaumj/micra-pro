using Microsoft.AspNetCore.Http;

namespace MicraPro.Auth.Domain.Interfaces;

public interface IOAuthProviderService
{
    Task<string> HandleAuthRequestAsync(HttpRequest request);
}
