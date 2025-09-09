using System.Security.Claims;
using MicraPro.Auth.DataDefinition;
using MicraPro.Auth.Domain.Interfaces;
using MicraPro.Auth.Domain.Services;
using Moq;

namespace MicraPro.Auth.Domain.Test;

public class AuthorizationServiceTest
{
    private AuthorizationService<PermissionDummy> CreateService()
    {
        var mock = new Mock<IRolePermissionService<PermissionDummy>>();
        mock.Setup(m => m.GetPermissionsForRole("Role1"))
            .Returns([PermissionDummy.Permission1, PermissionDummy.Permission2]);
        mock.Setup(m => m.GetPermissionsForRole("Role2")).Returns([PermissionDummy.Permission3]);
        return new AuthorizationService<PermissionDummy>(mock.Object);
    }

    // Don't make private, will be used by mock
    public enum PermissionDummy
    {
        Permission1,
        Permission2,
        Permission3,
    }

    private ClaimsPrincipal CreateClaims(IEnumerable<string> roles) =>
        new([new ClaimsIdentity(roles.Select(r => new Claim(ClaimTypes.Role, r)))]);

    [Fact]
    public void HasPermissionsTest()
    {
        var service = CreateService();
        var claims = CreateClaims(["Role1"]);
        Assert.True(
            service.HasPermissions(
                claims,
                [PermissionDummy.Permission1, PermissionDummy.Permission2]
            )
        );
        Assert.True(service.HasPermissions(claims, [PermissionDummy.Permission1]));
        Assert.True(service.HasPermissions(claims, [PermissionDummy.Permission2]));
        Assert.False(
            service.HasPermissions(
                claims,
                [PermissionDummy.Permission1, PermissionDummy.Permission3]
            )
        );
        Assert.False(
            service.HasPermissions(
                claims,
                [PermissionDummy.Permission2, PermissionDummy.Permission3]
            )
        );
        Assert.False(service.HasPermissions(claims, [PermissionDummy.Permission3]));
    }
}
