using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.ScaleManagement.Domain.StorageAccess;

public class ScaleDb : IEntity
{
    public Guid Id { get; }
    public string Identifier { get; }
    public string Name { get; set; }
    public string ImplementationType { get; }

    private ScaleDb(Guid id, string identifier, string name, string implementationType)
    {
        Id = id;
        Identifier = identifier;
        Name = name;
        ImplementationType = implementationType;
    }

    public ScaleDb(string identifier, string name, string implementationType)
        : this(Guid.NewGuid(), identifier, name, implementationType) { }
}
