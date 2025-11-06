using MicraPro.Machine.Domain.BluetoothAccess;
using MicraPro.Machine.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MicraPro.Machine.Domain.Services;

public class MachineConnectionFactory(
    IBluetoothService bluetoothService,
    ILogger<MachineConnection> logger
) : IMachineConnectionFactory
{
    public async Task<IMachineConnection> CreateAsync(string id, CancellationToken ct)
    {
        var connection = new MachineConnection(await bluetoothService.ConnectAsync(id, ct), logger);
        await connection.AuthenticateAsync(ct);
        connection.SubscribeToStandby();
        return connection;
    }
}
