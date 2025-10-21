namespace MicraPro.BeanManagement.DataDefinition;

public interface IGrinderSettings
{
    Task<double> GetGrinderOffset(CancellationToken ct);
    Task SetGrinderOffset(double grinderOffset, CancellationToken ct);
}
