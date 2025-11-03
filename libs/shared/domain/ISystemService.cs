namespace MicraPro.Shared.Domain;

public interface ISystemService
{
    public record Wifi(string Ssid, bool PasswordRequired);

    Task<bool> ShutdownAsync(CancellationToken ct);
    Task<bool> RebootAsync(CancellationToken ct);
    Task<string?> GetConnectedWifiAsync(CancellationToken ct);
    Task<Wifi[]> ScanWifiAsync(CancellationToken ct);
    Task<bool> ConnectWifiAsync(string ssid, string? password, CancellationToken ct);
    Task<bool> DisconnectWifiAsync(string ssid, CancellationToken ct);
}
