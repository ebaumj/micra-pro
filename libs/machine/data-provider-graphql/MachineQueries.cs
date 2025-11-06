using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using MicraPro.Machine.DataDefinition;
using MicraPro.Machine.DataDefinition.ValueObjects;
using MicraPro.Machine.DataProviderGraphQl.Services;
using MicraPro.Machine.DataProviderGraphQl.Types;

namespace MicraPro.Machine.DataProviderGraphQl;

[QueryType]
public static class MachineQueries
{
    public static Task<Boilers> GetBoilers(
        [Service] IMachineService service,
        CancellationToken ct
    ) => service.Machine().GetBoilersAsync(ct);

    public static async Task<SmartStandbyApi> GetSmartStandby(
        [Service] IMachineService service,
        CancellationToken ct
    ) => SmartStandbyApi.FromDomain(await service.Machine().GetSmartStandbyAsync(ct));

    public static async Task<MachineState> GetMachineState(
        [Service] IMachineService service,
        CancellationToken ct
    )
    {
        try
        {
            return await service.Machine().IsStandby.FirstAsync().ToTask(ct)
                ? MachineState.Standby
                : MachineState.Running;
        }
        catch
        {
            return MachineState.NotConnected;
        }
    }

    public static Task<IMachineService.MachineScanResult[]> GetScanResults(
        [Service] MachineScanContainerService service,
        CancellationToken ct
    ) => service.Results.FirstAsync().ToTask(ct);

    public static Task<bool> GetIsScanning(
        [Service] MachineScanContainerService service,
        CancellationToken ct
    ) => service.IsScanning.FirstAsync().ToTask(ct);
}
