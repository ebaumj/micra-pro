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
    private static readonly HttpClient Client = new();
    private string? _wifi;

    public string SystemVersion => options.Value.SystemVersion;
    public bool AllowUpdates => options.Value.AllowUpdates;

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
            if (!AllowUpdates)
                throw new Exception("Installing Updates not allowed!");

            var filePath = Path.Combine(
                options.Value.UpdateDestination,
                options.Value.UpdateFileName
            );
            var tempFilePath = filePath + ".tmp";
            if (!Directory.Exists(options.Value.UpdateDestination))
                Directory.CreateDirectory(options.Value.UpdateDestination);

            using var response = await Client.GetAsync(
                link,
                HttpCompletionOption.ResponseHeadersRead,
                ct
            );
            response.EnsureSuccessStatusCode();
            await using (
                var fileStream = new FileStream(
                    tempFilePath,
                    FileMode.Create,
                    FileAccess.ReadWrite,
                    FileShare.None,
                    81920,
                    true
                )
            )
            {
                await using var httpStream = await response.Content.ReadAsStreamAsync(ct);
                await httpStream.CopyToAsync(fileStream, ct);
                fileStream.Position = 0;
                var hash = await SHA256.HashDataAsync(fileStream, ct);

                using var rsa = RSA.Create();
                rsa.ImportFromPem(await File.ReadAllTextAsync(options.Value.UpdatePublicKey, ct));
                var formatter = new RSAPKCS1SignatureDeformatter(rsa);
                formatter.SetHashAlgorithm(nameof(SHA256));

                if (!formatter.VerifySignature(hash, Convert.FromBase64String(signature)))
                {
                    fileStream.Close();
                    File.Delete(tempFilePath);
                    throw new Exception("Invalid signature");
                }
            }

            if (File.Exists(filePath))
                File.Delete(filePath);
            File.Move(tempFilePath, filePath);
            File.Delete(tempFilePath);

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
