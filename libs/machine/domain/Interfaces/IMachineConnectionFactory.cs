using MicraPro.Machine.Domain.BluetoothAccess;

namespace MicraPro.Machine.Domain.Interfaces;

public interface IMachineConnectionFactory
{
    public Task<IMachineConnection> CreateAsync(string id, CancellationToken ct);
}
