using System.Diagnostics;
using MicraPro.Shared.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MicraPro.Shared.Infrastructure;

public class SystemService(
    IOptions<SharedInfrastructureOptions> options,
    ILogger<SystemService> logger
) : ISystemService
{
    private async Task<string> Bash(string file, string cmd)
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = file,
                Arguments = cmd,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
            },
        };
        process.Start();
        var result = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();
        return result;
    }

    public async Task<bool> ShutdownAsync(CancellationToken ct) =>
        (await Bash("/sbin/shutdown", "now")).Contains("The system will power off now!");

    public async Task<bool> RebootAsync(CancellationToken ct) =>
        (await Bash("/sbin/reboot", "now")).Contains("The system will reboot now!");

    public async Task<string?> GetConnectedWifiAsync(CancellationToken ct) =>
        (await Bash("/sbin/wpa_cli", $"status -i {options.Value.WifiAdapter}"))
            .Split('\n')
            .Select(line => line.Trim().Split('='))
            .Where(line => line.Length == 2)
            .ToDictionary(words => words[0], words => words[1])
            .TryGetValue("ssid", out var ssid)
            ? ssid
            : null;

    public async Task<ISystemService.Wifi[]> ScanWifiAsync(CancellationToken ct)
    {
        await Bash("/sbin/wpa_cli", $"scan -i {options.Value.WifiAdapter}");
        await Task.Delay(1000, ct);
        return (await Bash("/sbin/wpa_cli", $"scan_results -i {options.Value.WifiAdapter}"))
            .Split('\n')
            .Skip(1)
            .Select(line => line.Split('\t'))
            .Where(line => line.Length >= 5)
            .Select(words =>
                (
                    Ssid: words[4],
                    Flags: words[3]
                        .Split("][")
                        .Select(f => f.Replace("[", "").Replace("]", ""))
                        .ToArray()
                )
            )
            .Where(network =>
            {
                if (network.Ssid.Length < 1)
                    return false;
                if (!network.Flags.Contains("ESS"))
                    return false;
                return network.Flags.Length <= 1
                    || network.Flags.Any(flag => flag.Contains("WPA") && flag.Contains("PSK"));
            })
            .Select(network => new ISystemService.Wifi(network.Ssid, network.Flags.Length > 1))
            .Distinct()
            .ToArray();
    }

    public async Task<bool> ConnectWifiAsync(string ssid, string? password, CancellationToken ct)
    {
        try
        {
            var response = await Bash(
                "/sbin/wpa_cli",
                $"remove_network all -i {options.Value.WifiAdapter}"
            );
            if (response.Trim() != "OK")
                throw new Exception($"Failed to remove networks: {response}");
            response = (
                await Bash("/sbin/wpa_cli", $"add_network -i {options.Value.WifiAdapter}")
            ).Trim();
            if (response.Trim() == "FAIL")
                throw new Exception($"Failed to add network: {response}");
            var networkIndex = int.Parse(response.Trim());
            response = await Bash(
                "/sbin/wpa_cli",
                $"set_network {networkIndex} ssid \"\\\"{ssid}\\\"\" -i {options.Value.WifiAdapter}"
            );
            if (response.Trim() != "OK")
                throw new Exception($"Failed to set network ssid: {response}");
            if (password != null)
            {
                response = await Bash(
                    "/sbin/wpa_cli",
                    $"set_network {networkIndex} psk \"\\\"{password}\\\"\" -i {options.Value.WifiAdapter}"
                );
                if (response.Trim() != "OK")
                    throw new Exception($"Failed to set network psk: {response}");
            }
            else
            {
                response = await Bash(
                    "/sbin/wpa_cli",
                    $"set_network {networkIndex} key_mgmt NONE -i {options.Value.WifiAdapter}"
                );
                if (response.Trim() != "OK")
                    throw new Exception($"Failed to set network key_mgmt: {response}");
            }
            response = await Bash(
                "/sbin/wpa_cli",
                $"enable_network {networkIndex} -i {options.Value.WifiAdapter}"
            );
            if (response.Trim() != "OK")
                throw new Exception($"Failed to enable network: {response}");
            response = await Bash(
                "/sbin/wpa_cli",
                $"select_network {networkIndex} -i {options.Value.WifiAdapter}"
            );
            if (response.Trim() != "OK")
                throw new Exception($"Failed to select network: {response}");
            await Task.Delay(1000, ct);
            return true;
        }
        catch (Exception e)
        {
            logger.LogWarning(e.Message);
            return false;
        }
    }
}
