using MicraPro.Cleaning.DataDefinition.ValueObjects;
using MicraPro.Cleaning.Domain.Services;

namespace MicraPro.Cleaning.Domain.Test.Services;

public class CleaningDefaultsProviderTest
{
    private static readonly CleaningCycle[] Default =
    [
        new(TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(30)),
        new(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(3)),
        new(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(3)),
        new(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(3)),
        new(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(3)),
        new(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(3)),
        new(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(3)),
        new(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(3)),
        new(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(3)),
        new(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(3)),
        new(TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(3)),
    ];

    [Fact]
    public void DefaultsTest()
    {
        Assert.True(Default.SequenceEqual(new CleaningDefaultsProvider().DefaultSequence));
    }
}
