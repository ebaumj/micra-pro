using MicraPro.Cleaning.DataDefinition.ValueObjects;

namespace MicraPro.Cleaning.Domain.Interfaces;

public interface ICleaningDefaultsProvider
{
    CleaningCycle[] DefaultSequence { get; }
}
