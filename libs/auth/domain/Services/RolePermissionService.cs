using MicraPro.Auth.DataDefinition;
using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.Auth.Domain.Services;

public class RolePermissionService : IRolePermissionService<Permission>
{
    private readonly Dictionary<string, IEnumerable<string>> _includedRoles = new()
    {
        { AccessRoles.Admin, [AccessRoles.User] },
    };

    private readonly Dictionary<string, IEnumerable<Permission>> _permissions = new()
    {
        {
            AccessRoles.User,
            [Permission.ReadScales, Permission.ReadConfiguration, Permission.TestConnection]
        },
        { AccessRoles.Admin, [Permission.WriteScales, Permission.WriteConfiguration] },
    };

    public IEnumerable<Permission> GetPermissionsForRole(string role)
    {
        List<Permission> permissions = [];
        if (_permissions.TryGetValue(role, out var permission))
            permissions.AddRange(permission);
        if (_includedRoles.TryGetValue(role, out var includedRole))
            includedRole
                .Where(r => _permissions.ContainsKey(r))
                .SelectMany(r => _permissions[r])
                .ForEach(p =>
                {
                    if (!permissions.Contains(p))
                        permissions.Add(p);
                });
        return permissions;
    }
}
