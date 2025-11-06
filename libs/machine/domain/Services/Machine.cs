using System.Text.Json;
using MicraPro.Machine.DataDefinition;
using MicraPro.Machine.DataDefinition.ValueObjects;
using MicraPro.Machine.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MicraPro.Machine.Domain.Services;

public class Machine(IMachineConnection machineConnection) : IMachine
{
    private static readonly IReadOnlyDictionary<SmartStandby.SmartStandbyMode, string> StandbyMode =
        new Dictionary<SmartStandby.SmartStandbyMode, string>
        {
            { SmartStandby.SmartStandbyMode.LastBrew, "LastBrewing" },
            { SmartStandby.SmartStandbyMode.PowerOn, "PowerOn" },
        };
    private static readonly IReadOnlyDictionary<int, int> SteamLevelTemperature = new Dictionary<
        int,
        int
    >
    {
        { 1, 100 },
        { 2, 101 },
        { 3, 102 },
    };

    internal static IEnumerable<SmartStandby.SmartStandbyMode> StandbyModeDictionaryTest =>
        StandbyMode.Select(kvp => kvp.Key);

    public IObservable<bool> IsStandby => machineConnection.IsStandby;

    public Task SetStandbyAsync(bool standby, CancellationToken ct)
    {
        try
        {
            return machineConnection.WriteValueAsync(
                "MachineChangeMode",
                new { mode = standby ? "StandBy" : "BrewingMode" },
                ct
            );
        }
        catch (Exception e)
        {
            throw new MachineAccessException("Failed to set Standby", e);
        }
    }

    public async Task<SmartStandby?> GetSmartStandbyAsync(CancellationToken ct)
    {
        try
        {
            var modeDictionary = StandbyMode.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            var data = JsonSerializer.Deserialize<SmartStandbyDataRaw>(
                await machineConnection.ReadValueAsync(
                    IMachineConnection.ReadSetting.SmartStandBy,
                    ct
                )
            );
            if (data == null || !modeDictionary.TryGetValue(data.mode, out var mode))
                throw new InvalidDataException();
            return data.enabled ? new SmartStandby(TimeSpan.FromMinutes(data.minutes), mode) : null;
        }
        catch (Exception e)
        {
            throw new MachineAccessException("Failed to get Smart Standby", e);
        }
    }

    public async Task<Boilers> GetBoilersAsync(CancellationToken ct)
    {
        try
        {
            var data = JsonSerializer.Deserialize<BoilerDataRaw[]>(
                await machineConnection.ReadValueAsync(IMachineConnection.ReadSetting.Boilers, ct)
            );
            var coffeeBoiler = data?.FirstOrDefault(b => b.id == "CoffeeBoiler1");
            var steamBoiler = data?.FirstOrDefault(b => b.id == "SteamBoiler");
            if (coffeeBoiler is null || steamBoiler is null)
                throw new InvalidDataException();
            return new Boilers(
                new CoffeeBoiler(coffeeBoiler.isEnabled, coffeeBoiler.target, coffeeBoiler.current),
                new SteamBoiler(
                    steamBoiler.isEnabled,
                    TemperatureToSteamLevel(steamBoiler.target),
                    steamBoiler.current
                )
            );
        }
        catch (Exception e)
        {
            throw new MachineAccessException("Failed to get Boilers", e);
        }
    }

    public Task SetSmartStandbyAsync(SmartStandby? standby, CancellationToken ct)
    {
        try
        {
            return machineConnection.WriteValueAsync(
                "SettingSmartStandby",
                new
                {
                    minutes = standby?.Time.TotalMinutes ?? 0,
                    mode = StandbyMode[standby?.Mode ?? SmartStandby.SmartStandbyMode.LastBrew],
                    enabled = standby != null,
                },
                ct
            );
        }
        catch (Exception e)
        {
            throw new MachineAccessException("Failed set Smart Standby", e);
        }
    }

    public Task SetBoilerTargetTemperatureAsync(int temperature, CancellationToken ct)
    {
        try
        {
            return machineConnection.WriteValueAsync(
                "SettingBoilerTarget",
                new { identifier = "CoffeeBoiler1", value = temperature },
                ct
            );
        }
        catch (Exception e)
        {
            throw new MachineAccessException("Failed set Coffee Temperature", e);
        }
    }

    public Task SetSteamLevelAsync(int level, CancellationToken ct)
    {
        try
        {
            if (!SteamLevelTemperature.TryGetValue(level, out var temperature))
                throw new InvalidDataException();
            return machineConnection.WriteValueAsync(
                "SettingBoilerTarget",
                new { identifier = "SteamBoiler", value = temperature },
                ct
            );
        }
        catch (Exception e)
        {
            throw new MachineAccessException("Failed set Steam Level", e);
        }
    }

    public Task SetSteamBoilerEnabledAsync(bool enabled, CancellationToken ct)
    {
        try
        {
            return machineConnection.WriteValueAsync(
                "SettingBoilerEnabled",
                new { identifier = "SteamBoiler", value = enabled },
                ct
            );
        }
        catch (Exception e)
        {
            throw new MachineAccessException("Failed set Steam Boiler Enabled", e);
        }
    }

    private int TemperatureToSteamLevel(int temperature) =>
        temperature <= SteamLevelTemperature[1] ? 1
        : temperature >= SteamLevelTemperature[3] ? 3
        : 2;

    // ReSharper disable line InconsistentNaming
    private record BoilerDataRaw(string id, bool isEnabled, int target, int current);

    // ReSharper disable line InconsistentNaming
    private record SmartStandbyDataRaw(string mode, int minutes, bool enabled);
}
