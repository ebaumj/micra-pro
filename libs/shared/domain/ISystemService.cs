namespace MicraPro.Shared.Domain;

public interface ISystemService
{
    public record Wifi(string Ssid, bool PasswordRequired);

    public string SystemVersion { get; }
    public bool AllowUpdates { get; }

    Task<bool> ShutdownAsync(CancellationToken ct);
    Task<bool> RebootAsync(CancellationToken ct);
    Task<string?> GetConnectedWifiAsync(CancellationToken ct);
    Task<Wifi[]> ScanWifiAsync(CancellationToken ct);
    Task<bool> ConnectWifiAsync(string ssid, string? password, CancellationToken ct);
    Task<bool> DisconnectWifiAsync(string ssid, CancellationToken ct);
    Task<bool> InstallUpdateAsync(string link, string signature, CancellationToken ct);
}
