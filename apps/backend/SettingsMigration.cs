using System.Text.Json.Nodes;

namespace MicraPro.Backend;

public static class SettingsMigration
{
    private const string MigrationFileName = "appsettings.Migration.json";
    private const string SettingsFileName = "appsettings.json";
    private const string VersionKey = "MicraPro.Shared.Infrastructure.SystemVersion";

    public static void MigrateSettings()
    {
        try
        {
            if (!File.Exists(MigrationFileName))
                return;
            var settings = JsonNode.Parse(File.ReadAllText(SettingsFileName))?.AsObject();
            var migration = JsonNode.Parse(File.ReadAllText(MigrationFileName))?.AsObject();
            if (settings == null || migration == null)
                return;
            File.WriteAllText(SettingsFileName, Merge(settings, migration, null).ToString());
            File.Delete(MigrationFileName);
        }
        catch
        {
            // Settings can not be merged
        }
    }

    private static JsonObject Merge(JsonObject settings, JsonObject migration, string? parent)
    {
        foreach (var (key, value) in migration)
        {
            var nestedKey = parent == null ? $"{key}" : $"{parent}.{key}";
            if (settings.ContainsKey(key))
            {
                if (value is JsonObject obj && settings[key] is JsonObject set)
                    settings[key] = Merge(set, obj, nestedKey);
                else if (value?.GetType() != settings[key]?.GetType() || nestedKey == VersionKey)
                    settings[key] = value?.DeepClone();
            }
            else
                settings.Add(key, value?.DeepClone());
        }
        return settings;
    }
}
