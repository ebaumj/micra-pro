using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MicraPro.AssetManagement.Infrastructure.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace MicraPro.AssetManagement.Infrastructure.Services;

public class TokenCreatorService(
    IOptions<AssetManagementInfrastructureOptions> options,
    IOptions<AssetManagementInfrastructureRuntimeOptions> runtimeOptions
) : ITokenCreatorService
{
    private const string RefreshTokenSubject = "__refresh_token__";

    private readonly SymmetricSecurityKey _key = new(
        runtimeOptions.Value.RemoteAssetServerPrivateKey
    );

    private readonly TimeSpan _tokenLifeTime = TimeSpan.FromMinutes(
        double.Parse(options.Value.JwtTokenLifeTimeInMinutes)
    );

    private readonly TimeSpan _uploadTokenLifeTime = TimeSpan.FromMinutes(
        double.Parse(options.Value.JwtUploadTokenLifeTimeInMinutes)
    );

    private readonly TimeSpan _webhookLifeTime = TimeSpan.FromMinutes(60);

    private string CreateAccessToken(TimeSpan lifetime, IEnumerable<Claim> claims) =>
        new JwtSecurityTokenHandler().WriteToken(
            new JwtSecurityToken(
                new JwtHeader(new SigningCredentials(_key, SecurityAlgorithms.HmacSha256)),
                new JwtPayload(
                    options.Value.JwtIssuer,
                    options.Value.Audience,
                    claims,
                    null,
                    DateTime.UtcNow.Add(lifetime)
                )
            )
        );

    private static IEnumerable<Claim> CreateClaims(string subject) =>
        [new(JwtRegisteredClaimNames.Sub, subject)];

    public string GenerateUploadAccessToken(Guid assetId) =>
        CreateAccessToken(_uploadTokenLifeTime, CreateClaims(assetId.ToString()));

    public string GenerateAccessToken(Guid assetId) =>
        CreateAccessToken(_tokenLifeTime, CreateClaims(assetId.ToString()));

    public string GenerateWebhookAccessToken(string subject) =>
        CreateAccessToken(_webhookLifeTime, CreateClaims(subject));

    public string GenerateRefreshToken() =>
        CreateAccessToken(_webhookLifeTime, CreateClaims(RefreshTokenSubject));

    public string GenerateAccessToken() => CreateAccessToken(_tokenLifeTime, []);
}
