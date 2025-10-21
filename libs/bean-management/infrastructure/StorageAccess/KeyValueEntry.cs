namespace MicraPro.BeanManagement.Infrastructure.StorageAccess;

public class KeyValueEntry(string key, string jsonValue)
{
    public string Key { get; private set; } = key;
    public string JsonValue { get; set; } = jsonValue;
};
