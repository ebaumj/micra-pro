using MicraPro.Auth.DataDefinition;
using MicraPro.Auth.Domain.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Moq;

namespace MicraPro.Auth.Domain.Test;

public class JwtCreatorServiceTest
{
    private static readonly string[] RolesForUser1 = ["Admin", "User"];
    private static readonly string[] RolesForUser2 = ["User"];

    private static readonly AuthDomainOptions JwtConfiguration = new()
    {
        JwtIssuer = "Test",
        JwtValidIssuers = ["Test"],
        JwtTokenLifeTimeInMinutes = "0.05",
        JwtRefreshTokenLifeTimeInMinutes = "1",
    };

    private static readonly AuthDomainRuntimeOptions JwtRuntimeOptions = new()
    {
        PrivateKey = Enumerable.Range(15, 32).Select(n => (byte)n).ToArray(),
        Audience = "MicraPro",
    };

    private static JwtCreatorService CreateService()
    {
        var configurationMock = new Mock<IOptions<AuthDomainOptions>>();
        configurationMock.Setup(o => o.Value).Returns(JwtConfiguration);
        var runtimeOptionsMock = new Mock<IOptions<AuthDomainRuntimeOptions>>();
        runtimeOptionsMock.Setup(o => o.Value).Returns(JwtRuntimeOptions);
        return new JwtCreatorService(
            configurationMock.Object,
            runtimeOptionsMock.Object,
            Mock.Of<ILogger<JwtCreatorService>>()
        );
    }

    private static TokenValidationParameters ValidationParameters =>
        JwtCreatorService.CreateAccessValidationParameters(
            JwtRuntimeOptions,
            JwtConfiguration.JwtValidIssuers
        );

    [Fact]
    public async Task CreateValidTokenTest()
    {
        var tokenValidationResult = await new JsonWebTokenHandler().ValidateTokenAsync(
            CreateService().CreateAccessToken("", []),
            ValidationParameters
        );
        Assert.True(tokenValidationResult.IsValid);
    }

    [Fact]
    public async Task CreateExpiredTokenTest()
    {
        var token = CreateService().CreateAccessToken("", []);
        await Task.Delay(
            TimeSpan
                .FromMinutes(double.Parse(JwtConfiguration.JwtTokenLifeTimeInMinutes))
                .Add(TimeSpan.FromSeconds(1))
        );
        var tokenValidationResult = await new JsonWebTokenHandler().ValidateTokenAsync(
            token,
            ValidationParameters
        );
        Assert.False(tokenValidationResult.IsValid);
    }

    [Fact]
    public void CreateUser1TokenTest()
    {
        var token = new JsonWebTokenHandler().ReadJsonWebToken(
            CreateService().CreateAccessToken("User1", RolesForUser1)
        );
        Assert.True(
            token
                .Claims.Where(c => c.Type == CustomJwtClaimTypes.Roles)
                .Select(c => c.Value)
                .SequenceEqual(RolesForUser1)
        );
        Assert.Equal(
            "User1",
            token.Claims.FirstOrDefault(claim => claim.Type == CustomJwtClaimTypes.UserId)?.Value
        );
    }

    [Fact]
    public void CreateUser2TokenTest()
    {
        var token = new JsonWebTokenHandler().ReadJsonWebToken(
            CreateService().CreateAccessToken("User2", RolesForUser2)
        );
        Assert.True(
            token
                .Claims.Where(c => c.Type == CustomJwtClaimTypes.Roles)
                .Select(c => c.Value)
                .SequenceEqual(RolesForUser2)
        );
        Assert.Equal(
            "User2",
            token.Claims.FirstOrDefault(claim => claim.Type == CustomJwtClaimTypes.UserId)?.Value
        );
    }

    [Fact]
    public void CreateRefreshTokenTest()
    {
        var token = new JsonWebTokenHandler().ReadJsonWebToken(
            CreateService().CreateRefreshToken("User1", RolesForUser1)
        );
        Assert.Contains(
            AccessRoles.RefreshToken,
            token.Claims.Where(c => c.Type == CustomJwtClaimTypes.Roles).Select(c => c.Value)
        );
        Assert.True(
            token
                .Claims.Where(c => c.Type == CustomJwtClaimTypes.PersistedRoles)
                .Select(c => c.Value)
                .SequenceEqual(RolesForUser1)
        );
        Assert.Equal(
            "User1",
            token.Claims.FirstOrDefault(claim => claim.Type == CustomJwtClaimTypes.UserId)?.Value
        );
    }

    [Fact]
    public void ValidateRefreshTokenTest()
    {
        var service = CreateService();
        var token = service.CreateRefreshToken("User1", RolesForUser1);
        Assert.True(service.ValidateRefreshToken(token));
    }

    [Fact]
    public void GetUserNameFromRefreshTokenTest()
    {
        var service = CreateService();
        var token = service.CreateRefreshToken("User1", RolesForUser1);
        Assert.Equal("User1", service.GetUsernameFromRefreshToken(token));
    }

    [Fact]
    public void GetPersistedRolesFromRefreshTokenTest()
    {
        var service = CreateService();
        var token = service.CreateRefreshToken("User1", RolesForUser1);
        Assert.True(RolesForUser1.SequenceEqual(service.GetPersistedRolesFromRefreshToken(token)));
    }
}
