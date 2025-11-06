namespace MicraPro.Machine.DataDefinition;

public class MachineAccessException(string message, Exception inner) : Exception(message)
{
    public Exception ExceptionDetails => inner;
}
