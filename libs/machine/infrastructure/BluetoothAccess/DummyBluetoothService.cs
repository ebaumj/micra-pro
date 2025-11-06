using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using MicraPro.Machine.Domain.BluetoothAccess;
using IBluetoothService = MicraPro.Machine.Domain.BluetoothAccess.IBluetoothService;

namespace MicraPro.Machine.Infrastructure.BluetoothAccess;

public class DummyBluetoothService : IBluetoothService
{
    private class DataContainer
    {
        public int SmartStandbyMinutes { get; set; } = 0;
        public string SmartStandbyMode { get; set; } = "LastBrewing";
        public bool SmartStandbyEnabled { get; set; } = false;
        public string Mode { get; set; } = "BrewingMode";
        public int SteamBoilerTemperature { get; set; } = 100;
        public int CoffeeBoilerTemperature { get; set; } = 93;
        public bool SteamBoilerEnabled { get; set; } = true;
    }

    private class DummyAuthCharacteristic : IBluetoothCharacteristic
    {
        public Task<string> ReadAsync(CancellationToken ct) => throw new NotImplementedException();

        public Task WriteAsync(string data, CancellationToken ct) =>
            data == "MyToken" ? Task.CompletedTask : throw new Exception("Invalid key");
    }

    private class DummyTokenCharacteristic : IBluetoothCharacteristic
    {
        public Task<string> ReadAsync(CancellationToken ct) => Task.FromResult("MyToken");

        public Task WriteAsync(string data, CancellationToken ct) =>
            throw new NotImplementedException();
    }

    private class DummyReadCharacteristic(DataContainer container) : IBluetoothCharacteristic
    {
        private string _dataName = "";

        public Task<string> ReadAsync(CancellationToken ct) =>
            Task.FromResult(
                _dataName switch
                {
                    "machineMode\0" => container.Mode,
                    "boilers\0" => "[{\"id\":\"CoffeeBoiler1\",\"isEnabled\":true,\"target\":"
                        + container.CoffeeBoilerTemperature
                        + ",\"current\":"
                        + container.CoffeeBoilerTemperature
                        + "},{\"id\":\"SteamBoiler\",\"isEnabled\":"
                        + (container.SteamBoilerEnabled ? "true" : "false")
                        + ",\"target\":"
                        + container.SteamBoilerTemperature
                        + ",\"current\":"
                        + container.SteamBoilerTemperature
                        + "}]",
                    "smartStandBy\0" => "{\"minutes\":"
                        + container.SmartStandbyMinutes
                        + ",\"mode\":\""
                        + container.SmartStandbyMode
                        + "\",\"enabled\":"
                        + (container.SmartStandbyEnabled ? "true" : "false")
                        + "}",
                    _ => throw new Exception("Invalid key"),
                }
            );

        public Task WriteAsync(string data, CancellationToken ct)
        {
            _dataName = data;
            return Task.CompletedTask;
        }
    }

    private class DummyWriteCharacteristic(DataContainer container) : IBluetoothCharacteristic
    {
        private record Data(string name, JsonElement parameter);

        private record MachineChangeMode(string mode);

        private record SettingSmartStandby(int minutes, string mode, bool enabled);

        private record SettingBoilerTarget(string identifier, int value);

        private record SettingBoilerEnabled(string identifier, bool value);

        public Task<string> ReadAsync(CancellationToken ct) => throw new NotImplementedException();

        public Task WriteAsync(string data, CancellationToken ct)
        {
            var dataObject = JsonSerializer.Deserialize<Data>(data.Replace("\0", ""));
            switch (dataObject?.name)
            {
                case "MachineChangeMode":
                    var machineChangeMode = dataObject.parameter.Deserialize<MachineChangeMode>();
                    if (machineChangeMode == null)
                        break;
                    container.Mode = machineChangeMode.mode;
                    break;
                case "SettingSmartStandby":
                    var settingSmartStandby =
                        dataObject.parameter.Deserialize<SettingSmartStandby>();
                    if (settingSmartStandby == null)
                        break;
                    container.SmartStandbyMinutes = settingSmartStandby.minutes;
                    container.SmartStandbyMode = settingSmartStandby.mode;
                    container.SmartStandbyEnabled = settingSmartStandby.enabled;
                    break;
                case "SettingBoilerTarget":
                    var settingBoilerTarget =
                        dataObject.parameter.Deserialize<SettingBoilerTarget>();
                    if (settingBoilerTarget == null)
                        break;
                    switch (settingBoilerTarget.identifier)
                    {
                        case "CoffeeBoiler1":
                            container.CoffeeBoilerTemperature = settingBoilerTarget.value;
                            break;
                        case "SteamBoiler":
                            container.SteamBoilerTemperature = settingBoilerTarget.value;
                            break;
                    }
                    break;
                case "SettingBoilerEnabled":
                    var settingBoilerEnabled =
                        dataObject.parameter.Deserialize<SettingBoilerEnabled>();
                    if (settingBoilerEnabled == null)
                        break;
                    switch (settingBoilerEnabled.identifier)
                    {
                        case "SteamBoiler":
                            container.SteamBoilerEnabled = settingBoilerEnabled.value;
                            break;
                    }
                    break;
            }
            return Task.CompletedTask;
        }
    }

    private class DummyConnection : IBluetoothConnection
    {
        private static readonly DataContainer DataContainer = new();
        public IBluetoothCharacteristic Read { get; } = new DummyReadCharacteristic(DataContainer);
        public IBluetoothCharacteristic Write { get; } =
            new DummyWriteCharacteristic(DataContainer);
        public IBluetoothCharacteristic Auth { get; } = new DummyAuthCharacteristic();
        public IBluetoothCharacteristic Token { get; } = new DummyTokenCharacteristic();

        public Task DisconnectAsync(CancellationToken ct) => Task.CompletedTask;
    }

    private readonly Subject<IBluetoothService.ScanResult> _devices = new();
    private readonly BehaviorSubject<bool> _isScanning = new(false);
    public IObservable<IBluetoothService.ScanResult> DetectedDevices => _devices;
    public IObservable<bool> IsScanning => _isScanning;

    public async Task DiscoverAsync(TimeSpan discoveryTime, CancellationToken ct)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(1), ct);
        _isScanning.OnNext(true);
        Observable
            .FromAsync(() => DiscoverSequence(discoveryTime, ct))
            .Subscribe(_ => _isScanning.OnNext(false));
    }

    private async Task DiscoverSequence(TimeSpan discoveryTime, CancellationToken ct)
    {
        try
        {
            await Task.Delay(TimeSpan.FromSeconds(2), ct);
            _devices.OnNext(new IBluetoothService.ScanResult("Dummy Machine", "DummyId"));
            await Task.Delay(discoveryTime - TimeSpan.FromSeconds(2), ct);
        }
        catch (TaskCanceledException)
        {
            // just cancel the delay
        }
    }

    public Task<IBluetoothConnection> ConnectAsync(string bluetoothId, CancellationToken ct)
    {
        if (bluetoothId != "DummyId")
            throw new Exception("bluetoothId must be DummyId");
        return Task.FromResult<IBluetoothConnection>(new DummyConnection());
    }
}
