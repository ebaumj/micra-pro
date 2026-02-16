using System.Diagnostics;
using System.Security.Cryptography;
using MicraPro.Shared.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MicraPro.Shared.Infrastructure;

public class SystemService(
    IOptions<SharedInfrastructureOptions> options,
    ILogger<SystemService> logger
) : ISystemService, IWifiEnableService
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

    public string SystemVersion => options.Value.SystemVersion;
    public bool AllowUpdates => options.Value.AllowUpdates;

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
            password ??= "";
            var response = await Bash(
                "/usr/bin/nmcli",
                $"device wifi connect \"{ssid}\" password \"{password}\" ifname \"{options.Value.WifiAdapter}\""
            );
            if (response.StartsWith("Error"))
                throw new Exception($"Failed to connect network: {response}");
            await Task.Delay(1000, ct);
            return true;
        }
        catch (Exception e)
        {
            logger.LogWarning(e.Message);
            return false;
        }
    }

    public async Task<bool> DisconnectWifiAsync(string ssid, CancellationToken ct)
    {
        try
        {
            var response = await Bash("/usr/bin/nmcli", $"connection delete id \"{ssid}\"");
            if (response.StartsWith("Error"))
                throw new Exception($"Failed to connect network: {response}");
            await Task.Delay(TimeSpan.FromMilliseconds(500), ct);
            return true;
        }
        catch (Exception e)
        {
            logger.LogWarning(e.Message);
            return false;
        }
    }

    public async Task<bool> InstallUpdateAsync(string link, string signature, CancellationToken ct)
    {
        try
        {
            if (!AllowUpdates)
                throw new Exception("Installing Updates not allowed!");
            using var client = new HttpClient();
            var stream = await (await client.GetAsync(link, ct)).Content.ReadAsStreamAsync(ct);
            var fileData = new byte[stream.Length];
            await stream.ReadExactlyAsync(fileData, 0, fileData.Length, ct);
            var hash = SHA256.HashData(fileData);
            using var rsa = RSA.Create();
            rsa.ImportFromPem(await File.ReadAllTextAsync(options.Value.UpdatePublicKey, ct));
            var formatter = new RSAPKCS1SignatureDeformatter(rsa);
            formatter.SetHashAlgorithm(nameof(SHA256));
            if (!formatter.VerifySignature(hash, Convert.FromBase64String(signature)))
                throw new Exception("Invalid signature");
            if (!Directory.Exists(options.Value.UpdateDestination))
                Directory.CreateDirectory(options.Value.UpdateDestination);
            var filePath = Path.Combine(
                options.Value.UpdateDestination,
                options.Value.UpdateFileName
            );
            if (File.Exists(filePath))
                File.Delete(filePath);
            var fs = File.Create(filePath);
            await fs.WriteAsync(fileData, ct);
            fs.Close();
        }
        catch (Exception e)
        {
            logger.LogWarning("Failed to install update: {e}", e);
            return false;
        }
        return await RebootAsync(ct);
    }

    public async Task<bool> EnableWifi(CancellationToken ct)
    {
        try
        {
            var response = await Bash("/usr/bin/nmcli", "radio wifi on");
            if (response.StartsWith("Error"))
                throw new Exception($"Failed to set Wifi on: {response}");
            return true;
        }
        catch (Exception e)
        {
            logger.LogWarning(e.Message);
            return false;
        }
    }
}
