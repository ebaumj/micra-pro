namespace MicraPro.Shared.Infrastructure;

public interface IWifiEnableService
{
    Task<bool> EnableWifi(CancellationToken ct);
}
