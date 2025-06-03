namespace MicraPro.Shared.Infrastructure;

public class ConfigurationEntry(string key, string jsonValue)
{
    public string Key { get; private set; } = key;
    public string JsonValue { get; set; } = jsonValue;
};
