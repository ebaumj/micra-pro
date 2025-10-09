using System.Security.Claims;

namespace MicraPro.Auth.Domain;

public static class ClaimsPrincipalExtensions
{
    public static IEnumerable<string> GetStringClaimValues(
        this ClaimsPrincipal claims,
        string claimType
    )
    {
        return claims.FindAll(c => c.Type == claimType).Select(c => c.Value);
    }
}
