using MicraPro.Auth.DataDefinition;

namespace MicraPro.Auth.Domain.Services;

internal class NoAccountManagementService : IAccountManagementService
{
    public bool CheckPassword(string username, string password) => false;

    public IEnumerable<string> GetRolesForUser(string username) => [];
}
