using System.Reactive.Linq;
using System.Reactive.Subjects;
using Linux.Bluetooth;
using Linux.Bluetooth.Extensions;
using MicraPro.Shared.Domain.BluetoothAccess;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace MicraPro.Shared.Infrastructure.BluetoothAccess;

public class BluetoothService(IOptions<SharedInfrastructureOptions> configuration)
    : IBluetoothService,
        IHostedService
{
    private Adapter? _bleAdapter;
    private Adapter BleAdapter => _bleAdapter ?? throw new Exception("Bluetooth adapter not found");

    private static readonly TimeSpan DeviceSearchTimeout = TimeSpan.FromSeconds(5);

    private readonly BehaviorSubject<bool> _isScanning = new(false);
    private readonly ReplaySubject<IBluetoothService.BluetoothScanResult> _discoveredDevices =
        new();
    public IObservable<bool> IsScanning => _isScanning;
    public IObservable<IBluetoothService.BluetoothScanResult> DetectedDevices =>
        _discoveredDevices.Distinct(d => d.DeviceId);

    private async Task DeviceChangeEventHandlerAsync(Adapter sender, DeviceFoundEventArgs eventArgs)
    {
        var device = (Device?)eventArgs.Device;
        if (device == null)
            return;
        var properties = await device.GetPropertiesAsync();
        if (properties.Address is null)
            return;
        _discoveredDevices.OnNext(
            new IBluetoothService.BluetoothScanResult(
                properties.Name ?? properties.Address,
                properties.Address,
                properties.UUIDs ?? []
            )
        );
    }

    private async Task StopDiscoveryAsync()
    {
        await BleAdapter.StopDiscoveryAsync();
        _isScanning.OnNext(false);
    }

    public async Task DiscoverAsync(TimeSpan discoveryTime, CancellationToken ct)
    {
        await BleAdapter.StartDiscoveryAsync();
        _isScanning.OnNext(true);
        var subscription = Observable
            .Timer(discoveryTime)
            .Select(_ => Observable.FromAsync(async () => await StopDiscoveryAsync()))
            .Merge()
            .Subscribe(_ => { });
        ct.Register(() =>
            Observable
                .FromAsync(async () => await StopDiscoveryAsync())
                .Subscribe(_ => subscription.Dispose())
        );
    }

    public async Task<IBleDeviceConnection> ConnectDeviceAsync(
        string deviceId,
        CancellationToken ct
    )
    {
        await BleAdapter.StartDiscoveryAsync();
        Device? device;
        do
        {
            device = (Device?)
                await BleAdapter.GetDeviceAsync(deviceId).WaitAsync(DeviceSearchTimeout, ct);
        } while (device == null);
        await BleAdapter.StopDiscoveryAsync();
        await device.ConnectAsync();
        return new BleDeviceConnection(device);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _bleAdapter = await BlueZManager.GetAdapterAsync(configuration.Value.BluetoothAdapter);
        BleAdapter.DeviceFound += DeviceChangeEventHandlerAsync;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
