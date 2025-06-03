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
    private readonly SymmetricSecurityKey _key = new(
        runtimeOptions.Value.RemoteAssetServerPrivateKey
    );

    private readonly TimeSpan _tokenLifeTime = TimeSpan.FromMinutes(
        double.Parse(options.Value.JwtTokenLifeTimeInMinutes)
    );

    private readonly TimeSpan _uploadTokenLifeTime = TimeSpan.FromMinutes(
        double.Parse(options.Value.JwtUploadTokenLifeTimeInMinutes)
    );

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

    private IEnumerable<Claim> CreateClaims(Guid assetId) =>
        [new(JwtRegisteredClaimNames.Sub, assetId.ToString())];

    public string GenerateUploadAccessToken(Guid assetId) =>
        CreateAccessToken(_uploadTokenLifeTime, CreateClaims(assetId));

    public string GenerateAccessToken(Guid assetId) =>
        CreateAccessToken(_tokenLifeTime, CreateClaims(assetId));

    public string GenerateAccessToken() => CreateAccessToken(_tokenLifeTime, []);
}
