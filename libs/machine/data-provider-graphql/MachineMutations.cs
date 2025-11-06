using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using MicraPro.Machine.DataDefinition;
using MicraPro.Machine.DataDefinition.ValueObjects;
using MicraPro.Machine.DataProviderGraphQl.Services;
using MicraPro.Machine.DataProviderGraphQl.Types;

namespace MicraPro.Machine.DataProviderGraphQl;

[MutationType]
public static class MachineMutations
{
    public static async Task<bool> ConnectMachine(
        string deviceId,
        [Service] IMachineService service,
        CancellationToken ct
    )
    {
        await service.ConnectAsync(deviceId, ct);
        return true;
    }

    public static async Task<bool> DisconnectMachine(
        [Service] IMachineService service,
        CancellationToken ct
    )
    {
        await service.DisconnectAsync(ct);
        return true;
    }

    public static async Task<bool> SetStandby(
        bool standby,
        [Service] IMachineService service,
        CancellationToken ct
    )
    {
        await service.Machine().SetStandbyAsync(standby, ct);
        return await service.Machine().IsStandby.FirstAsync().ToTask(ct);
    }

    public static async Task<SmartStandbyApi> SetSmartStandby(
        SmartStandbyApi standby,
        [Service] IMachineService service,
        CancellationToken ct
    )
    {
        await service.Machine().SetSmartStandbyAsync(standby.ToDomain(), ct);
        return SmartStandbyApi.FromDomain(await service.Machine().GetSmartStandbyAsync(ct));
    }

    public static async Task<Boilers> SetCoffeeTemperature(
        int temperature,
        [Service] IMachineService service,
        CancellationToken ct
    )
    {
        await service.Machine().SetBoilerTargetTemperatureAsync(temperature, ct);
        return await service.Machine().GetBoilersAsync(ct);
    }

    public static async Task<Boilers> SetSteamLevel(
        int level,
        [Service] IMachineService service,
        CancellationToken ct
    )
    {
        await service.Machine().SetSteamLevelAsync(level, ct);
        return await service.Machine().GetBoilersAsync(ct);
    }

    public static async Task<Boilers> SetSteamEnabled(
        bool enabled,
        [Service] IMachineService service,
        CancellationToken ct
    )
    {
        await service.Machine().SetSteamBoilerEnabledAsync(enabled, ct);
        return await service.Machine().GetBoilersAsync(ct);
    }

    public static Task<bool> ScanDevices(
        [Service] MachineScanContainerService containerService,
        CancellationToken _
    )
    {
        containerService.StartScanning();
        return Task.FromResult(true);
    }

    public static Task<bool> StopScanning(
        [Service] MachineScanContainerService containerService,
        CancellationToken _
    )
    {
        containerService.StopScanning();
        return Task.FromResult(true);
    }
}
