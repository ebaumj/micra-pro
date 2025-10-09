using System.Security.Claims;

namespace MicraPro.Auth.Domain.Interfaces;

public interface IJwtCreatorService
{
    string CreateAccessToken(string username, IEnumerable<string> roles);
    string CreateRefreshToken(string username, IEnumerable<string> roles);
    bool ValidateRefreshToken(string token);
    ClaimsPrincipal? ValidateAccessToken(string token);
    string GetUsernameFromRefreshToken(string token);
    IEnumerable<string> GetPersistedRolesFromRefreshToken(string token);
}
