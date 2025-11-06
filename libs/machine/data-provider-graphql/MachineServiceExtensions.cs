using MicraPro.Machine.DataDefinition;
using MicraPro.Machine.DataProviderGraphQl.Types;

namespace MicraPro.Machine.DataProviderGraphQl;

public static class MachineServiceExtensions
{
    public static IMachine Machine(this IMachineService service)
    {
        var machine = service.Machine;
        if (machine == null)
            throw new MachineNotFoundException();
        return machine;
    }
}
