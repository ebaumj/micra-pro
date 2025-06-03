namespace MicraPro.ScaleManagement.Domain.BluetoothAccess;

public interface IBleDeviceConnection : IDisposable
{
    Task<IBleService> GetServiceAsync(Guid serviceId, CancellationToken ct);
    Task<IBleService[]> GetServicesAsync(CancellationToken ct);
    Task Disconnect(CancellationToken ct);
}
