namespace MicraPro.BrewByWeight.DataDefinition.ValueObjects;

public abstract class BrewByWeightException : Exception
{
    public class ScaleConnectionFailed : BrewByWeightException;

    public class ScaleDisconnected : BrewByWeightException;

    public class BrewSwitchAccessFailed : BrewByWeightException;

    public class BrewServiceNotReady : BrewByWeightException;

    public class UnknownError : BrewByWeightException;
}
