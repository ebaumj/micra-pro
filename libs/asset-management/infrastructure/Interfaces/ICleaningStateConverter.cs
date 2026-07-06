using MicraPro.Cleaning.DataDefinition.ValueObjects;

namespace MicraPro.AssetManagement.Infrastructure.Interfaces;

public interface ICleaningStateConverter
{
    object Convert(CleaningCycle[] cycles, CleaningState.Started state);
    object Convert(CleaningCycle[] cycles, CleaningState.Finished state);
    object Convert(CleaningCycle[] cycles, CleaningState.Cancelled state);
    object Convert(CleaningCycle[] cycles, CleaningState.Failed state);
    object Convert(CleaningCycle[] cycles, CleaningState.Running state);
}
