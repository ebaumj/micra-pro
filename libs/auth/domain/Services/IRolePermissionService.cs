namespace MicraPro.Auth.Domain.Services;

public interface IRolePermissionService<TPermission>
    where TPermission : Enum
{
    IEnumerable<TPermission> GetPermissionsForRole(string role);
}
