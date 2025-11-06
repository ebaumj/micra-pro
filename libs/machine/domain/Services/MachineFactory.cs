using MicraPro.Machine.DataDefinition;
using MicraPro.Machine.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MicraPro.Machine.Domain.Services;

public class MachineFactory : IMachineFactory
{
    public IMachine Create(IMachineConnection machineConnection) => new Machine(machineConnection);
}
