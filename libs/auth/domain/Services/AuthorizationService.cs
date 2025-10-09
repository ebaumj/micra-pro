using System.Collections.Immutable;
using System.Security.Claims;
using MicraPro.Auth.DataDefinition;
using MicraPro.Auth.Domain.Interfaces;

namespace MicraPro.Auth.Domain.Services;

public class AuthorizationService<TPermission>(
    IRolePermissionService<TPermission> rolePermissionService
) : IAuthorizationService<TPermission>
    where TPermission : Enum
{
    public bool HasPermissions(ClaimsPrincipal user, IEnumerable<TPermission> permissions) =>
        permissions.All(p =>
            user.GetStringClaimValues(ClaimTypes.Role)
                .SelectMany(rolePermissionService.GetPermissionsForRole)
                .Distinct()
                .ToImmutableSortedSet()
                .Contains(p)
        );
}
