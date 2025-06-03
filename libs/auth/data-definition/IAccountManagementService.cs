namespace MicraPro.Auth.DataDefinition;

public interface IAccountManagementService
{
    bool CheckPassword(string username, string password);
    IEnumerable<string> GetRolesForUser(string username);
}
