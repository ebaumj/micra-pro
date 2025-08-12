namespace MicraPro.SerialCommunication.Domain.HardwareAccess;

public interface ISerialDataService
{
    Task SendAsync(byte[] data, CancellationToken ct);
    IObservable<byte[]> Received { get; }
}
