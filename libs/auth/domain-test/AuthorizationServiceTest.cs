using System.Security.Claims;
using MicraPro.Auth.DataDefinition;
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
    public void HasPermissionAsyncTest()
    {
        var service = CreateService();
        var claims = CreateClaims(["Role1"]);
        Assert.True(service.HasPermission(claims, PermissionDummy.Permission1));
        Assert.True(service.HasPermission(claims, PermissionDummy.Permission2));
        Assert.False(service.HasPermission(claims, PermissionDummy.Permission3));
        claims = CreateClaims(["Role2"]);
        Assert.False(service.HasPermission(claims, PermissionDummy.Permission1));
        Assert.False(service.HasPermission(claims, PermissionDummy.Permission2));
        Assert.True(service.HasPermission(claims, PermissionDummy.Permission3));
    }

    [Fact]
    public void HasPermissionsAsyncTest()
    {
        var service = CreateService();
        var claims = CreateClaims(["Role1"]);
        Assert.True(service.HasPermission(claims, PermissionDummy.Permission1));
        Assert.True(service.HasPermission(claims, PermissionDummy.Permission2));
        Assert.False(service.HasPermission(claims, PermissionDummy.Permission3));
        claims = CreateClaims(["Role2"]);
        Assert.False(service.HasPermission(claims, PermissionDummy.Permission1));
        Assert.False(service.HasPermission(claims, PermissionDummy.Permission2));
        Assert.True(service.HasPermission(claims, PermissionDummy.Permission3));
    }

    [Fact]
    public void GetPermissionsAsync()
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

    [Fact]
    public void AssertPermissionAsyncTest()
    {
        var service = CreateService();
        var claims = CreateClaims(["Role1"]);
        service.AssertPermission(claims, PermissionDummy.Permission1);
        service.AssertPermission(claims, PermissionDummy.Permission2);
        Assert.Throws<IAuthorizationService<PermissionDummy>.AuthorizationException>(() =>
            service.AssertPermission(claims, PermissionDummy.Permission3)
        );
        claims = CreateClaims(["Role2"]);
        Assert.Throws<IAuthorizationService<PermissionDummy>.AuthorizationException>(() =>
            service.AssertPermission(claims, PermissionDummy.Permission1)
        );
        Assert.Throws<IAuthorizationService<PermissionDummy>.AuthorizationException>(() =>
            service.AssertPermission(claims, PermissionDummy.Permission2)
        );
        service.AssertPermission(claims, PermissionDummy.Permission3);
    }

    [Fact]
    public void AssertPermissionsAsyncTest()
    {
        var service = CreateService();
        var claims = CreateClaims(["Role1"]);
        service.AssertPermissions(
            claims,
            [PermissionDummy.Permission1, PermissionDummy.Permission2]
        );
        service.AssertPermissions(claims, [PermissionDummy.Permission1]);
        service.AssertPermissions(claims, [PermissionDummy.Permission2]);
        Assert.Throws<IAuthorizationService<PermissionDummy>.AuthorizationException>(() =>
            service.AssertPermissions(
                claims,
                [PermissionDummy.Permission1, PermissionDummy.Permission3]
            )
        );
        Assert.Throws<IAuthorizationService<PermissionDummy>.AuthorizationException>(() =>
            service.AssertPermissions(
                claims,
                [PermissionDummy.Permission2, PermissionDummy.Permission3]
            )
        );
        Assert.Throws<IAuthorizationService<PermissionDummy>.AuthorizationException>(() =>
            service.AssertPermissions(claims, [PermissionDummy.Permission3])
        );
    }
}
