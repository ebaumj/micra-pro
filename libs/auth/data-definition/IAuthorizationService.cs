using System.Collections.Immutable;
using System.Security.Claims;

namespace MicraPro.Auth.DataDefinition;

public interface IAuthorizationService<TPermission>
    where TPermission : Enum
{
    bool HasPermission(ClaimsPrincipal user, TPermission permission);
    bool HasPermissions(ClaimsPrincipal user, IEnumerable<TPermission> permissions);
    IImmutableSet<TPermission> GetPermissions(ClaimsPrincipal user);
    void AssertPermission(ClaimsPrincipal? user, TPermission permission);
    void AssertPermissions(ClaimsPrincipal? user, IEnumerable<TPermission> permissions);

    class AuthorizationException() : Exception("You are not authorized to perform this action.");
}
