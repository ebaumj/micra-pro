using System.Collections.Immutable;

namespace MicraPro.Auth.DataDefinition.Test;

public class PermissionsTest
{
    // Do not remove or change entries!!!
    private readonly ImmutableDictionary<int, Permission> _permissions = new[]
    {
        // Common
        (0, Permission.TestConnection),
        // Scale Management
        (1000, Permission.ReadScales),
        (1001, Permission.WriteScales),
        //
        // Configuration Strings
        (7000, Permission.ReadConfiguration),
        (7001, Permission.WriteConfiguration),
    }.ToImmutableDictionary(tuple => tuple.Item1, tuple => tuple.Item2);

    [Fact]
    public void BackwardCompatibilityTest()
    {
        var allPermissions = Enum.GetValues<Permission>();
        foreach (var permission in allPermissions)
        {
            Assert.True(_permissions.ContainsKey((int)permission));
            Assert.Equal(_permissions[(int)permission], permission);
        }
    }
}
