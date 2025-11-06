namespace MicraPro.Machine.Domain.DatabaseAccess;

public interface IMachineRepository
{
    Task<string?> GetCurrentMachineAsync(CancellationToken ct);
    Task SetCurrentMachineAsync(string entry, CancellationToken ct);
    Task RemoveCurrentMachineAsync(CancellationToken ct);
}
