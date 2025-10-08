using HotChocolate.Execution;
using MicraPro.ScaleManagement.DataDefinition;
using MicraPro.ScaleManagement.DataDefinition.ValueObjects;
using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.ScaleManagement.DataProviderGraphQl;

[SubscriptionType]
public static class ScaleManagementSubscriptions
{
    [Subscribe(With = nameof(SubscribeToScanResult))]
    public static BluetoothScale ScanResult([EventMessage] BluetoothScale result) => result;

    public static ValueTask<ISourceStream<BluetoothScale>> SubscribeToScanResult(
        [Service] IScaleService scaleService,
        CancellationToken _
    ) => ValueTask.FromResult(scaleService.DetectedScales.ToSourceStream());

    [Subscribe(With = nameof(SubscribeToIsScanning))]
    public static bool IsScanning([EventMessage] bool result) => result;

    public static ValueTask<ISourceStream<bool>> SubscribeToIsScanning(
        [Service] IScaleService scaleService,
        CancellationToken _
    ) => ValueTask.FromResult(scaleService.IsScanning.ToSourceStream());
}
