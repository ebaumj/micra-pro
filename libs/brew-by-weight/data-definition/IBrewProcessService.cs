namespace MicraPro.BrewByWeight.DataDefinition;

public interface IBrewProcessService
{
    Task<bool> IsBrewProcessRunning { get; }
}
