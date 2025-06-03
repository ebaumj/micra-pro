using MicraPro.Shared.UtilsDotnet;

namespace MicraPro.BeanManagement.Domain.StorageAccess;

public class RoasteryDb : IEntity
{
    public Guid Id { get; }
    public string Name { get; set; }
    public string Location { get; set; }

    public ICollection<BeanDb> Beans { get; } = new List<BeanDb>();

    private RoasteryDb(Guid id, string name, string location)
    {
        Id = id;
        Name = name;
        Location = location;
    }

    public RoasteryDb(string name, string location)
        : this(Guid.NewGuid(), name, location) { }
}
