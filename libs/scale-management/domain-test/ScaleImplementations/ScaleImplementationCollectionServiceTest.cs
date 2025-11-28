using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using MicraPro.ScaleManagement.Domain.ScaleImplementations;
using Moq;

namespace MicraPro.ScaleManagement.Domain.Test.ScaleImplementations;

public class ScaleImplementationCollectionServiceTest
{
    private static readonly Type[] SupportedImplementations =
    [
        typeof(MicraPro.ScaleManagement.Domain.ScaleImplementations.BookooThemisMini.Scale),
        typeof(MicraPro.ScaleManagement.Domain.ScaleImplementations.Acaia.OldStyle.Scale),
        typeof(MicraPro.ScaleManagement.Domain.ScaleImplementations.Acaia.PyxisStyle.Scale),
    ];

    [Fact]
    public void ImplementationsBackwardsCompatibilityTest()
    {
        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(m => m.GetService(typeof(IEnumerable<IScaleImplementationFactory>)))
            .Returns(
                SupportedImplementations.Select(i =>
                    Mock.Of<IScaleImplementationFactory>(m =>
                        m.TypeName == i.FullName!
                        && m.IsScaleType(It.IsAny<BluetoothScanResult>()) == true
                    )
                )
            );
        var service = new ScaleImplementationCollectionService(serviceProviderMock.Object);
        var implementations = service.Implementations.Select(m => m.Name).ToArray();
        foreach (var implementationType in SupportedImplementations)
            Assert.Contains(implementationType.FullName, implementations);
    }
}
