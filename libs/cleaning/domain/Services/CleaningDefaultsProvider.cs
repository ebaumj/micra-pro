using MicraPro.Cleaning.DataDefinition.ValueObjects;
using MicraPro.Cleaning.Domain.Interfaces;

namespace MicraPro.Cleaning.Domain.Services;

public class CleaningDefaultsProvider : ICleaningDefaultsProvider
{
    public CleaningCycle[] DefaultSequence =>
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
}
