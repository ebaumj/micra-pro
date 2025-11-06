using MicraPro.Machine.DataDefinition;

namespace MicraPro.Machine.Domain.Interfaces;

public interface IMachineFactory
{
    public IMachine Create(IMachineConnection machineConnection);
}
