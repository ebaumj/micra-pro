using MicraPro.ScaleManagement.Domain.Services;

namespace MicraPro.ScaleManagement.Domain.Test.Services;

public class ScaleImplementationMemoryTest
{
    [Fact]
    public void ImplementationFoundTest()
    {
        var service = new ScaleImplementationMemoryService();
        service.SetImplementation("MyScale", "MyImplementation");
        Assert.Equal("MyImplementation", service.GetImplementation("MyScale"));
    }

    [Fact]
    public void ImplementationNotFoundTest()
    {
        var service = new ScaleImplementationMemoryService();
        Assert.Throws<KeyNotFoundException>(() => service.GetImplementation("MyScale"));
    }
}
