using InTheHand.Bluetooth;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using Microsoft.Extensions.Caching.Memory;

namespace MicraPro.ScaleManagement.Infrastructure.BluetoothAccess.InTheHand;

public class BluetoothService(IMemoryCache cache) : IBluetoothService
{
    private static string GetServiceImplementationCacheKey(string deviceId, Guid service) =>
        $"{typeof(BluetoothService).FullName}.Services.{deviceId}.{service}";

    public Task<string[]> ScanDevicesAsync(CancellationToken ct) => ScanDevicesAsync([], ct);

    public async Task<string[]> ScanDevicesAsync(
        IEnumerable<Guid> requiredServiceIds,
        CancellationToken ct
    )
    {
        var requiredServices = requiredServiceIds.ToArray();
        var allDevices = (
            await Bluetooth.ScanForDevicesAsync(CreateScanOptions(requiredServices), ct)
        )
            .Select(d => d.Id)
            .ToArray();
        foreach (var dev in allDevices)
        foreach (var service in requiredServices)
            cache.Set(dev, service);
        return allDevices;
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

    private static RequestDeviceOptions CreateScanOptions(IEnumerable<Guid> requiredServiceIds)
    {
        var filter = new BluetoothLEScanFilter();
        foreach (var id in requiredServiceIds)
            filter.Services.Add(id);
        return new RequestDeviceOptions
        {
            AcceptAllDevices = filter.Services.Count == 0,
            Filters = { filter },
        };
    }

    public async Task<IBleDeviceConnection> ConnectDeviceAsync(
        string deviceId,
        CancellationToken ct
    )
    {
        var device = await BluetoothDevice.FromIdAsync(deviceId);
        if (device == null)
            throw new Exception("Device not found");
        await device.Gatt.ConnectAsync();
        return new BleDeviceConnection(device);
    }
}
