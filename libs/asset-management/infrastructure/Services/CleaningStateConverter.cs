using MicraPro.AssetManagement.Infrastructure.Interfaces;
using MicraPro.Cleaning.DataDefinition.ValueObjects;

namespace MicraPro.AssetManagement.Infrastructure.Services;

public class CleaningStateConverter : ICleaningStateConverter
{
    private record CleaningData(object[] Cycles, object State);

    public object Convert(CleaningCycle[] cycles, CleaningState.Started state) =>
        new CleaningData(ConvertCycles(cycles), new { });

    public object Convert(CleaningCycle[] cycles, CleaningState.Finished state) =>
        new CleaningData(
            ConvertCycles(cycles),
            new { TotalTimeSeconds = state.TotalTime.TotalSeconds, state.TotalCycles }
        );

    public object Convert(CleaningCycle[] cycles, CleaningState.Cancelled state) =>
        new CleaningData(
            ConvertCycles(cycles),
            new { TotalTimeSeconds = state.TotalTime.TotalSeconds, state.TotalCycles }
        );

    public object Convert(CleaningCycle[] cycles, CleaningState.Failed state) =>
        new CleaningData(
            ConvertCycles(cycles),
            new { TotalTimeSeconds = state.TotalTime.TotalSeconds, state.TotalCycles }
        );

    public object Convert(CleaningCycle[] cycles, CleaningState.Running state) =>
        new CleaningData(
            ConvertCycles(cycles),
            new
            {
                TotalTimeSeconds = state.TotalTime.TotalSeconds,
                state.CycleNumber,
                CycleTimeSeconds = state.CycleTime.TotalSeconds,
            }
        );

    private static object[] ConvertCycles(CleaningCycle[] cycles) =>
        cycles
            .Select(c => new
            {
                PaddleOnTimeSeconds = c.PaddleOnTime.TotalSeconds,
                PaddleOffTimeSeconds = c.PaddleOffTime.TotalSeconds,
            })
            .Cast<object>()
            .ToArray();
}
