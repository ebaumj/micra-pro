namespace MicraPro.ScaleManagement.DataDefinition;

public interface IScale
{
    Task<IScaleConnection> ConnectAsync(CancellationToken ct);
}
