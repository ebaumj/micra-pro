using System.Text.Json.Nodes;
using KeyNotFoundException = GreenDonut.KeyNotFoundException;
using Path = System.IO.Path;

namespace MicraPro.Backend;

public static class SettingsMigration
{
    private const string MigrationFileName = "appsettings.Migration.json";
    private const string SettingsFileName = "appsettings.json";
    private const string VersionKey = "MicraPro.Shared.Infrastructure.SystemVersion";
    private const string FrontendLocationKey = "MicraPro.Backend.FrontendSourceLocation";
    private const string FrontendMigrationFileName = "appconfig.Migration.json";
    private const string FrontendSettingsFileName = "appconfig.json";

    public static void MigrateSettings()
    {
        Migrate(SettingsFileName, MigrationFileName, [VersionKey]);
        var frontendSettingsPath = Path.Combine(GetFrontendLocation(), FrontendSettingsFileName);
        var frontendMigrationPath = Path.Combine(GetFrontendLocation(), FrontendMigrationFileName);
        Migrate(frontendSettingsPath, frontendMigrationPath, []);
    }

    private static string GetFrontendLocation() =>
        FrontendLocationKey
            .Split('.')
            .Aggregate(
                JsonNode.Parse(File.ReadAllText(SettingsFileName))!,
                (node, key) => node[key] ?? throw new KeyNotFoundException(key)
            )
            .ToString();

    private static void Migrate(string settingsPath, string migrationPath, string[] updateKeys)
    {
        try
        {
            if (!File.Exists(migrationPath))
                return;
            var migration = JsonNode.Parse(File.ReadAllText(migrationPath))?.AsObject();
            var settings = File.Exists(settingsPath)
                ? JsonNode.Parse(File.ReadAllText(settingsPath))?.AsObject()
                : JsonNode.Parse(File.ReadAllText(migrationPath))?.AsObject();
            if (settings == null || migration == null)
                return;
            File.WriteAllText(
                settingsPath,
                Merge(settings, migration, null, updateKeys).ToString()
            );
            File.Delete(migrationPath);
        }
        catch
        {
            // Settings can not be merged
        }
    }

    private static JsonObject Merge(
        JsonObject settings,
        JsonObject migration,
        string? parent,
        string[] replaceKeys
    )
    {
        foreach (var (key, value) in migration)
        {
            var nestedKey = parent == null ? $"{key}" : $"{parent}.{key}";
            if (settings.ContainsKey(key))
            {
                if (value is JsonObject obj && settings[key] is JsonObject set)
                    settings[key] = Merge(set, obj, nestedKey, replaceKeys);
                else if (
                    value?.GetType() != settings[key]?.GetType()
                    || replaceKeys.Contains(nestedKey)
                )
                    settings[key] = value?.DeepClone();
            }
            else
                settings.Add(key, value?.DeepClone());
        }
        return settings;
    }
}
