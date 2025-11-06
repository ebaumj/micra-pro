using System.Reactive.Linq;
using HotChocolate.Execution;
using MicraPro.Machine.DataDefinition;
using MicraPro.Machine.DataProviderGraphQl.Services;
using MicraPro.Machine.DataProviderGraphQl.Types;
using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.Machine.DataProviderGraphQl;

[SubscriptionType]
public static class MachineSubscriptions
{
    [Subscribe(With = nameof(SubscribeToMachineState))]
    public static MachineState MachineState([EventMessage] MachineState state) => state;

    public static ValueTask<ISourceStream<MachineState>> SubscribeToMachineState(
        [Service] IMachineService machineService,
        CancellationToken _
    ) =>
        ValueTask.FromResult(
            machineService
                .MachineObservable.Select(m =>
                    m == null
                        ? Observable.Return(Types.MachineState.NotConnected)
                        : m.IsStandby.Select(s =>
                            s ? Types.MachineState.Standby : Types.MachineState.Running
                        )
                )
                .Merge()
                .ToSourceStream()
        );

    [Subscribe(With = nameof(SubscribeToScanResults))]
    public static IMachineService.MachineScanResult[] ScanResults(
        [EventMessage] IMachineService.MachineScanResult[] results
    ) => results;

    public static ValueTask<
        ISourceStream<IMachineService.MachineScanResult[]>
    > SubscribeToScanResults(
        [Service] MachineScanContainerService containerService,
        CancellationToken _
    ) => ValueTask.FromResult(containerService.Results.ToSourceStream());

    [Subscribe(With = nameof(SubscribeToIsScanning))]
    public static bool IsScanning([EventMessage] bool scanning) => scanning;

    public static ValueTask<ISourceStream<bool>> SubscribeToIsScanning(
        [Service] MachineScanContainerService containerService,
        CancellationToken _
    ) => ValueTask.FromResult(containerService.IsScanning.ToSourceStream());
}
