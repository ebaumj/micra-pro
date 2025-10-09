using MicraPro.Auth.DataDefinition;
using MicraPro.Auth.Domain.Interfaces;
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
            [
                Permission.TestConnection,
                Permission.ReadConfiguration,
                Permission.ReadScales,
                Permission.ReadRoasteries,
                Permission.ReadBeans,
                Permission.ReadRecipes,
                Permission.ReadAssets,
                Permission.BrewCoffee,
                Permission.ReadStatistics,
            ]
        },
        {
            AccessRoles.Admin,
            [
                Permission.SystemAccess,
                Permission.WriteConfiguration,
                Permission.WriteScales,
                Permission.WriteRoasteries,
                Permission.WriteBeans,
                Permission.WriteRecipes,
                Permission.WriteAssets,
                Permission.WriteStatistics,
            ]
        },
    };

    public IEnumerable<Permission> GetPermissionsForRole(string role)
    {
        List<Permission> permissions = [];
        if (_permissions.TryGetValue(role, out var permission))
            permissions.AddRange(permission);
        if (_includedRoles.TryGetValue(role, out var includedRole))
            foreach (
                var p in includedRole
                    .Where(r => _permissions.ContainsKey(r))
                    .SelectMany(r => _permissions[r])
            )
                if (!permissions.Contains(p))
                    permissions.Add(p);
        return permissions;
    }
}
