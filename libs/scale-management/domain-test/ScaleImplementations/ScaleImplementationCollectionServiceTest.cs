using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using MicraPro.ScaleManagement.Domain.ScaleImplementations;
using Moq;

namespace MicraPro.ScaleManagement.Domain.Test.ScaleImplementations;

public class ScaleImplementationCollectionServiceTest
{
    private static readonly Type[] SupportedImplementations =
    [
        typeof(MicraPro.ScaleManagement.Domain.ScaleImplementations.BookooThemisMini.Scale),
    ];

    [Fact]
    public void ImplementationsBackwardsCompatibilityTest()
    {
        var service = new ScaleImplementationCollectionService(Mock.Of<IBluetoothService>());
        var implementations = service.Implementations.Select(m => m.Name).ToArray();
        foreach (var implementationType in SupportedImplementations)
            Assert.Contains(implementationType.FullName, implementations);
    }
}
