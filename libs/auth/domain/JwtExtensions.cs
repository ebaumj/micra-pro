using System.IdentityModel.Tokens.Jwt;

namespace MicraPro.Auth.Domain;

internal static class JwtExtensions
{
    public static string Create(this JwtSecurityToken token)
    {
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
