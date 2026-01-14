using System.Security.Cryptography;
using MicraPro.Shared.Domain;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MicraPro.Shared.Infrastructure;

public class SystemServiceDummy(
    IOptions<SharedInfrastructureOptions> options,
    ILogger<SystemServiceDummy> logger
) : ISystemService
{
    private string? _wifi;

    public string SystemVersion => options.Value.SystemVersion;

    public Task<bool> ShutdownAsync(CancellationToken ct)
    {
        logger.LogInformation("shutdown");
        return Task.FromResult(false);
    }

    public Task<bool> RebootAsync(CancellationToken ct)
    {
        logger.LogInformation("reboot");
        return Task.FromResult(false);
    }

    public async Task<string?> GetConnectedWifiAsync(CancellationToken ct)
    {
        await Task.Delay(500, ct);
        return _wifi;
    }

    public async Task<ISystemService.Wifi[]> ScanWifiAsync(CancellationToken ct)
    {
        await Task.Delay(2000, ct);
        return
        [
            new ISystemService.Wifi("Dummy 1", true),
            new ISystemService.Wifi("Dummy 2", false),
        ];
    }

    public async Task<bool> ConnectWifiAsync(string ssid, string? password, CancellationToken ct)
    {
        await Task.Delay(500, ct);
        _wifi = ssid switch
        {
            "Dummy 1" when password == "dummy" => "Dummy 1",
            "Dummy 2" => "Dummy 2",
            _ => null,
        };
        return _wifi != null;
    }

    public async Task<bool> DisconnectWifiAsync(string ssid, CancellationToken ct)
    {
        await Task.Delay(200, ct);
        if (_wifi == ssid)
            _wifi = null;
        return true;
    }

    public async Task<bool> InstallUpdateAsync(string link, string signature, CancellationToken ct)
    {
        try
        {
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
            logger.LogInformation("Update Installed");
            return false;
        }
        catch (Exception e)
        {
            logger.LogWarning("Failed to install update: {e}", e);
            return false;
        }
    }
}
