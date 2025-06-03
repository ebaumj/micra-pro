using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MicraPro.Auth.DataDefinition;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MicraPro.Auth.Domain.Services;

internal class JwtCreatorService(
    IOptions<AuthDomainOptions> configuration,
    IOptions<AuthDomainRuntimeOptions> runtimeOptions,
    ILogger<JwtCreatorService> logger
) : IJwtCreatorService
{
    private readonly SymmetricSecurityKey _key = new(runtimeOptions.Value.PrivateKey);

    private readonly TimeSpan _tokenLifeTime = TimeSpan.FromMinutes(
        double.Parse(configuration.Value.JwtTokenLifeTimeInMinutes)
    );

    private readonly TimeSpan _refreshTokenLifeTime = TimeSpan.FromMinutes(
        double.Parse(configuration.Value.JwtRefreshTokenLifeTimeInMinutes)
    );

    public static TokenValidationParameters CreateAccessValidationParameters(
        AuthDomainRuntimeOptions runtimeOptions,
        IEnumerable<string> validIssuers
    ) =>
        new()
        {
            ValidIssuers = validIssuers,
            ValidateIssuer = true,
            ValidAudience = runtimeOptions.Audience,
            ValidateAudience = true,
            IssuerSigningKey = new SymmetricSecurityKey(runtimeOptions.PrivateKey),
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            LifetimeValidator = TokenLifetimeValidator,
        };

    private static bool TokenLifetimeValidator(
        DateTime? notBefore,
        DateTime? expires,
        SecurityToken tokenToValidate,
        TokenValidationParameters @param
    ) => expires > DateTime.UtcNow;

    public string CreateAccessToken(string username, IEnumerable<string> roles) =>
        new JwtSecurityToken(
            new JwtHeader(new SigningCredentials(_key, SecurityAlgorithms.HmacSha256)),
            new JwtPayload(
                configuration.Value.JwtIssuer,
                runtimeOptions.Value.Audience,
                roles
                    .Select(r => new Claim(CustomJwtClaimTypes.Roles, r))
                    .Concat([new Claim(CustomJwtClaimTypes.UserId, username)]),
                null,
                DateTime.UtcNow.Add(_tokenLifeTime)
            )
        ).Create();

    public string CreateRefreshToken(string username, IEnumerable<string> roles) =>
        new JwtSecurityToken(
            new JwtHeader(new SigningCredentials(_key, SecurityAlgorithms.HmacSha256)),
            new JwtPayload(
                configuration.Value.JwtIssuer,
                runtimeOptions.Value.Audience,
                roles
                    .Select(r => new Claim(CustomJwtClaimTypes.PersistedRoles, r))
                    .Concat([new Claim(CustomJwtClaimTypes.UserId, username)])
                    .Concat([new Claim(CustomJwtClaimTypes.Roles, AccessRoles.RefreshToken)]),
                null,
                DateTime.UtcNow.Add(_refreshTokenLifeTime)
            )
        ).Create();

    public bool ValidateRefreshToken(string token)
    {
        try
        {
            var claimsOfValidToken = new JwtSecurityTokenHandler().ValidateToken(
                token,
                CreateAccessValidationParameters(
                    runtimeOptions.Value,
                    [configuration.Value.JwtIssuer]
                ),
                out _
            );
            return claimsOfValidToken is not null
                && claimsOfValidToken.HasClaim(RoleClaimType, AccessRoles.RefreshToken)
                && claimsOfValidToken.HasClaim(claim => claim.Type == CustomJwtClaimTypes.UserId)
                && claimsOfValidToken.HasClaim(claim =>
                    claim.Type == CustomJwtClaimTypes.PersistedRoles
                );
        }
        catch (Exception e)
        {
            logger.LogWarning("Failed to validate Refresh Token {t}! Details: {e}", token, e);
            return false;
        }
    }

    public ClaimsPrincipal? ValidateAccessToken(string token)
    {
        try
        {
            return new JwtSecurityTokenHandler().ValidateToken(
                token,
                CreateAccessValidationParameters(
                    runtimeOptions.Value,
                    [configuration.Value.JwtIssuer]
                ),
                out _
            );
        }
        catch (Exception e)
        {
            logger.LogWarning("Failed to validate Access Token {t}! Details: {e}", token, e);
            return null;
        }
    }

    public string GetUsernameFromRefreshToken(string token)
    {
        var userId = new JwtSecurityTokenHandler()
            .ReadJwtToken(token)
            .Claims.FirstOrDefault(claim => claim.Type == CustomJwtClaimTypes.UserId)
            ?.Value;
        if (userId == null)
            throw new InvalidDataException("Missing UserId in Refresh Token");
        return userId;
    }

    public IEnumerable<string> GetPersistedRolesFromRefreshToken(string token) =>
        new JwtSecurityTokenHandler()
            .ReadJwtToken(token)
            .Claims.Where(claim => claim.Type == CustomJwtClaimTypes.PersistedRoles)
            .Select(claim => claim.Value);

    private const string RoleClaimType =
        "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
}
