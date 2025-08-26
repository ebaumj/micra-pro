namespace MicraPro.Shared.DataProviderGraphQl;

public class ConfigDoesNotExistException(string key) : Exception
{
    public string Key { get; } = key;
}
