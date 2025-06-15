namespace MicraPro.ScaleManagement.Domain.BluetoothAccess;

public interface IBleDeviceConnection : IDisposable
{
    Task<IBleService> GetServiceAsync(string serviceId, CancellationToken ct);
    Task Disconnect(CancellationToken ct);
}
