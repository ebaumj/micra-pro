namespace MicraPro.ScaleManagement.Domain.BluetoothAccess;

public interface IBluetoothService
{
    Task<string[]> ScanDevicesAsync(CancellationToken ct);
    Task<string[]> ScanDevicesAsync(IEnumerable<Guid> requiredServiceIds, CancellationToken ct);
    Task<bool> HasRequiredServiceIdsAsync(
        string deviceId,
        IEnumerable<Guid> requiredServiceIds,
        CancellationToken ct
    );
    Task<IBleDeviceConnection> ConnectDeviceAsync(string deviceId, CancellationToken ct);
}
