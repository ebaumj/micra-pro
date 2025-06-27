namespace MicraPro.Shared.Domain;

public interface ISystemService
{
    Task<bool> ShutdownAsync(CancellationToken ct);
    Task<bool> RebootAsync(CancellationToken ct);
}
