namespace MicraPro.Shared.Infrastructure.KeyValueStore;

internal class KeyValueEntry(string key, string jsonValue)
{
    public string Key { get; private set; } = key;
    public string JsonValue { get; set; } = jsonValue;
}
