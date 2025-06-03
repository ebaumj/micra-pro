using System.Collections.Immutable;
using System.Security.Claims;
using MicraPro.Auth.DataDefinition;

namespace MicraPro.Auth.Domain.Services;

public class AuthorizationService<TPermission>(
    IRolePermissionService<TPermission> rolePermissionService
) : IAuthorizationService<TPermission>
    where TPermission : Enum
{
    public bool HasPermission(ClaimsPrincipal user, TPermission permission) =>
        GetPermissions(user).Contains(permission);

    public bool HasPermissions(ClaimsPrincipal user, IEnumerable<TPermission> permissions)
    {
        var userPermissions = GetPermissions(user);
        return permissions.All(p => userPermissions.Contains(p));
    }

    public IImmutableSet<TPermission> GetPermissions(ClaimsPrincipal user) =>
        user.GetStringClaimValues(ClaimTypes.Role)
            .SelectMany(rolePermissionService.GetPermissionsForRole)
            .Distinct()
            .ToImmutableSortedSet();

    public void AssertPermission(ClaimsPrincipal? user, TPermission permission)
    {
        if (user == null)
            throw new IAuthorizationService<TPermission>.AuthorizationException();
        if (!HasPermission(user, permission))
            throw new IAuthorizationService<TPermission>.AuthorizationException();
    }

    public void AssertPermissions(ClaimsPrincipal? user, IEnumerable<TPermission> permissions)
    {
        if (user == null)
            throw new IAuthorizationService<TPermission>.AuthorizationException();
        if (!HasPermissions(user, permissions))
            throw new IAuthorizationService<TPermission>.AuthorizationException();
    }
}
