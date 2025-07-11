namespace MicraPro.ScaleManagement.DataDefinition;

public interface IScale
{
    Guid Id { get; }
    string Name { get; }
    Task<IScaleConnection> ConnectAsync(CancellationToken ct);
}
