using MicraPro.BrewByWeight.DataDefinition;
using MicraPro.BrewByWeight.DataDefinition.ValueObjects;

namespace MicraPro.AssetManagement.Infrastructure.Interfaces;

public interface IBrewStateConverter
{
    object Convert(IBrewProcess process, BrewByWeightTracking.Started tracking);
    object Convert(IBrewProcess process, BrewByWeightTracking.Running tracking);
    object Convert(IBrewProcess process, BrewByWeightTracking.Finished tracking);
    object Convert(IBrewProcess process, BrewByWeightTracking.Cancelled tracking);
    object Convert(IBrewProcess process, BrewByWeightTracking.Failed tracking);
    object Convert(IBrewByTimeProcess process, BrewByTimeTracking.Started tracking);
    object Convert(IBrewByTimeProcess process, BrewByTimeTracking.Running tracking);
    object Convert(IBrewByTimeProcess process, BrewByTimeTracking.Finished tracking);
    object Convert(IBrewByTimeProcess process, BrewByTimeTracking.Cancelled tracking);
    object Convert(IBrewByTimeProcess process, BrewByTimeTracking.Failed tracking);
}
