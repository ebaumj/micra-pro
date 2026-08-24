using System.Text.Json.Nodes;
using Path = System.IO.Path;

namespace MicraPro.Backend;

public static class SettingsMigration
{
    private const string MigrationFileName = "appsettings.Migration.json";
    private const string SettingsFileName = "appsettings.json";
    private const string VersionKey = "MicraPro.Shared.Infrastructure.SystemVersion";
    private const string FrontendMigrationFileName = "appconfig.Migration.json";
    private const string FrontendSettingsFileName = "appconfig.json";

    public static void MigrateSettings()
    {
        var basePath = AppContext.BaseDirectory;
        var settingsPath = Path.Combine(basePath, SettingsFileName);
        var migrationPath = Path.Combine(basePath, MigrationFileName);
        Migrate(settingsPath, migrationPath, [VersionKey]);
        if (TryGetFrontendLocation(settingsPath, out var frontendLocation))
        {
            var frontendSettingsPath = Path.Combine(frontendLocation, FrontendSettingsFileName);
            var frontendMigrationPath = Path.Combine(frontendLocation, FrontendMigrationFileName);
            Migrate(frontendSettingsPath, frontendMigrationPath, []);
        }
    }

    private static bool TryGetFrontendLocation(string settingsPath, out string location)
    {
        location = string.Empty;
        try
        {
            if (!File.Exists(settingsPath))
                return false;
            var json = JsonNode.Parse(File.ReadAllText(settingsPath));
            var node = json?["MicraPro"]?["Backend"]?["FrontendSourceLocation"];
            if (node != null)
            {
                location = node.ToString();
                return Directory.Exists(location)
                    ? true
                    : throw new DirectoryNotFoundException(location);
            }
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(
                $"[SettingsMigration] Failed to parse frontend location. {e.Message}"
            );
        }
        return false;
    }

    private static void Migrate(string settingsPath, string migrationPath, string[] updateKeys)
    {
        if (!File.Exists(migrationPath))
            return;
        try
        {
            var migration = JsonNode.Parse(File.ReadAllText(migrationPath))?.AsObject();
            var settings = File.Exists(settingsPath)
                ? JsonNode.Parse(File.ReadAllText(settingsPath))?.AsObject()
                : new JsonObject();
            if (settings == null || migration == null)
                return;
            File.WriteAllText(
                settingsPath,
                Merge(settings, migration, null, updateKeys).ToString()
            );
            File.Delete(migrationPath);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(
                $"[SettingsMigration] Failed to merge settings at {settingsPath}. Details: {e}"
            );
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
