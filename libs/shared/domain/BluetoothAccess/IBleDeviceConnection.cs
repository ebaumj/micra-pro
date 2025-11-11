namespace MicraPro.Shared.Domain.BluetoothAccess;

public interface IBleDeviceConnection
{
    Task<IBleCharacteristic> GetCharacteristicAsync(string characteristicId, CancellationToken ct);
    Task<IBleService> GetServiceAsync(string serviceId, CancellationToken ct);
    Task Disconnect(CancellationToken ct);
}
