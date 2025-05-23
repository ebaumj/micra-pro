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

    public static IEnumerable<T> GetEnumClaimValues<T>(
        this ClaimsPrincipal claims,
        string claimType
    )
        where T : Enum
    {
        return claims
            .FindAll(c => c.Type == claimType)
            .Where(c => int.TryParse(c.Value, out _))
            .Select(c => (T)(object)int.Parse(c.Value));
    }
}
