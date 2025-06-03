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

    // Configuration Strings
    ReadConfiguration = 7000,
    WriteConfiguration,
}
