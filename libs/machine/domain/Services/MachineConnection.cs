using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text.Json;
using MicraPro.Machine.Domain.BluetoothAccess;
using MicraPro.Machine.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MicraPro.Machine.Domain.Services;

public class MachineConnection(
    IBluetoothConnection bluetoothConnection,
    ILogger<MachineConnection> logger
) : IMachineConnection
{
    private static readonly TimeSpan StandbyPollInterval = TimeSpan.FromMilliseconds(250);
    private static readonly IReadOnlyDictionary<IMachineConnection.ReadSetting, string> Settings =
        new Dictionary<IMachineConnection.ReadSetting, string>
        {
            { IMachineConnection.ReadSetting.MachineCapabilities, "machineCapabilities" },
            { IMachineConnection.ReadSetting.MachineMode, "machineMode" },
            { IMachineConnection.ReadSetting.TankStatus, "tankStatus" },
            { IMachineConnection.ReadSetting.Boilers, "boilers" },
            { IMachineConnection.ReadSetting.SmartStandBy, "smartStandBy" },
        };

    internal static IEnumerable<IMachineConnection.ReadSetting> SettingsDictionaryTest =>
        Settings.Select(kvp => kvp.Key);

    private readonly BehaviorSubject<bool> _isStandby = new(false);
    private IDisposable _subscription = Disposable.Empty;

    public async Task<string> ReadValueAsync(
        IMachineConnection.ReadSetting readSetting,
        CancellationToken ct
    )
    {
        await WriteValueAsync(Settings[readSetting], bluetoothConnection.Read, ct);
        return await bluetoothConnection.Read.ReadAsync(ct);
    }

    public Task WriteValueAsync(string name, object data, CancellationToken ct) =>
        WriteValueAsync(
            JsonSerializer.Serialize(new { name, parameter = data }),
            bluetoothConnection.Write,
            ct
        );

    public Task DisconnectAsync(CancellationToken ct)
    {
        _subscription.Dispose();
        return bluetoothConnection.DisconnectAsync(ct);
    }

    public IObservable<bool> IsStandby => _isStandby;

    private Task WriteValueAsync(
        string value,
        IBluetoothCharacteristic characteristic,
        CancellationToken ct
    ) => characteristic.WriteAsync($"{value}\0", ct);

    public async Task AuthenticateAsync(CancellationToken ct)
    {
        var token = await bluetoothConnection.Token.ReadAsync(ct);
        await bluetoothConnection.Auth.WriteAsync(token, ct);
    }

    public void SubscribeToStandby()
    {
        _subscription = Observable
            .Interval(StandbyPollInterval)
            .Select(_ =>
                Observable.FromAsync(c =>
                    ReadValueAsync(IMachineConnection.ReadSetting.MachineMode, c)
                )
            )
            .Merge()
            .Subscribe(
                val => _isStandby.OnNext(val == "StandBy"),
                e => logger.LogError("Failed to read Machine Mode: {e}", e)
            );
    }
}
