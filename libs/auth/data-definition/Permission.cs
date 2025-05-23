namespace MicraPro.Auth.DataDefinition;

public enum Permission
{
    // Common
    TestConnection = 0,

    // Scale Management
    ReadScales = 1000,
    WriteScales,

    // Configuration Strings
    ReadConfiguration = 7000,
    WriteConfiguration,
}
