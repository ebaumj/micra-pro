namespace MicraPro.Auth.DataDefinition;

public enum Permission
{
    // Common
    TestConnection = 0,

    // Scale Management
    ReadScales = 1000,
    WriteScales,

    // Bean Management
    ReadRoasteries = 2000,
    WriteRoasteries,
    ReadBeans,
    WriteBeans,
    ReadRecipes,
    WriteRecipes,

    // Asset Management
    ReadAssets = 3000,
    WriteAssets,

    // Configuration Strings
    ReadConfiguration = 7000,
    WriteConfiguration,
}
