namespace MicraPro.Auth.DataDefinition;

public enum Permission
{
    // Common
    TestConnection = 0,
    SystemAccess,

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

    // Brew By Weight
    BrewCoffee = 4000,
    ReadStatistics,
    WriteStatistics,

    // Configuration Strings
    ReadConfiguration = 7000,
    WriteConfiguration,
}
