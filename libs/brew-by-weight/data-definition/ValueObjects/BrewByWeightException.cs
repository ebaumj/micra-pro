namespace MicraPro.BrewByWeight.DataDefinition.ValueObjects;

public abstract class BrewByWeightException : Exception
{
    public class ScaleConnectionFailed : BrewByWeightException;

    public class BrewServiceNotReady : BrewByWeightException;

    public class UnknownError : BrewByWeightException;
}
