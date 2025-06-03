using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using Linux.Bluetooth;
using Linux.Bluetooth.Extensions;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using MicraPro.Shared.UtilsDotnet;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace MicraPro.ScaleManagement.Infrastructure.BluetoothAccess.LinuxBluetooth;

public class BluetoothService(
    IMemoryCache cache,
    IOptions<ScaleManagementInfrastructureOptions> configuration
) : IBluetoothService, IHostedService
{
    private static string GetServiceImplementationCacheKey(string deviceId, Guid service) =>
        $"{typeof(BluetoothService).FullName}.Services.{deviceId}.{service}";

    private IAdapter1? _bleAdapter;
    private IAdapter1 BleAdapter =>
        _bleAdapter ?? throw new Exception("Bluetooth adapter not found");

    private readonly TimeSpan _scanTimeout = TimeSpan.FromSeconds(30);

    public Task<string[]> ScanDevicesAsync(CancellationToken ct) => ScanDevicesAsync([], ct);

    public async Task<string[]> ScanDevicesAsync(
        IEnumerable<Guid> requiredServiceIds,
        CancellationToken ct
    )
    {
        var requiredServices = requiredServiceIds.ToArray();
        await BleAdapter.StartDiscoveryAsync();
        await Observable.Timer(_scanTimeout).ToTask(ct);
        await BleAdapter.StopDiscoveryAsync();
        var devices = await BleAdapter.GetDevicesAsync();
        var services = requiredServices.ToArray();
        var ids = new List<string>();
        foreach (var dev in devices)
        {
            if (await HasRequiredServiceIds(services, dev))
                ids.Add(await dev.GetAddressAsync());
        }
        foreach (var dev in ids)
        foreach (var service in requiredServices)
            cache.Set(dev, service);
        return ids.ToArray();
    }

    public Task<bool> HasRequiredServiceIdsAsync(
        string deviceId,
        IEnumerable<Guid> requiredServiceIds,
        CancellationToken ct
    ) =>
        Task.FromResult(
            requiredServiceIds.All(s =>
                cache.TryGetValue(GetServiceImplementationCacheKey(deviceId, s), out _)
            )
        );

    private static async Task<bool> HasRequiredServiceIds(
        IEnumerable<Guid> requiredServiceIds,
        Device device
    )
    {
        var timeout = TimeSpan.FromSeconds(15);
        await device.ConnectAsync();
        await device.WaitForPropertyValueAsync("Connected", value: true, timeout);
        await device.WaitForPropertyValueAsync("ServicesResolved", value: true, timeout);
        var services = await (await device.GetServicesAsync()).SelectAsync(s => s.GetUUIDAsync());
        await device.DisconnectAsync();
        return requiredServiceIds.All(id =>
            services.FirstOrDefault(s => s == id.ToString()) != null
        );
    }

    public async Task<IBleDeviceConnection> ConnectDeviceAsync(
        string deviceId,
        CancellationToken ct
    )
    {
        var device = await BleAdapter.GetDeviceAsync(deviceId);
        if (device == null)
            throw new Exception("Device not found");
        await device.ConnectAsync();
        return new BleDeviceConnection(device);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _bleAdapter = await BlueZManager.GetAdapterAsync(
            configuration.Value.LinuxBluetoothAdapterName
        );
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
