namespace MicraPro.ScaleManagement.Domain.ScaleImplementations.AcaiaLunar;

public interface IConnectionLifetimeData
{
    public record DataFrame(int Command, byte[] Payload, byte Cs1, byte Cs2);

    void DisposeHeartbeat();
    void SetupHeartbeat(TimeSpan period, Func<CancellationToken, Task> action);
    DataFrame? MergeData(byte[] data);
    double CalculateFlow(double weight);
}
