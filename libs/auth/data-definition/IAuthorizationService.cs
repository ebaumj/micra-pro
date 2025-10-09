using System.Security.Claims;

namespace MicraPro.Auth.DataDefinition;

public interface IAuthorizationService<TPermission>
    where TPermission : Enum
{
    bool HasPermissions(ClaimsPrincipal user, IEnumerable<TPermission> permissions);

    class AuthorizationException() : Exception("You are not authorized to perform this action.");
}
