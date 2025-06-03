namespace MicraPro.ScaleManagement.Domain.BluetoothAccess;

public interface IBleDeviceConnection
{
    Task<IBleService> GetServiceAsync(string serviceId, CancellationToken ct);
    Task Disconnect(CancellationToken ct);
}
