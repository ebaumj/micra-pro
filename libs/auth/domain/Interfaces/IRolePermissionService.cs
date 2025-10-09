namespace MicraPro.Auth.Domain.Interfaces;

public interface IRolePermissionService<out TPermission>
    where TPermission : Enum
{
    IEnumerable<TPermission> GetPermissionsForRole(string role);
}
