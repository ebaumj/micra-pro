using System.Reactive.Linq;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using MicraPro.ScaleManagement.Domain.ScaleImplementations.BookooThemisMini;
using MicraPro.ScaleManagement.Domain.StorageAccess;
using Moq;

namespace MicraPro.ScaleManagement.Domain.Test.ScaleImplementations.BookooTemisMini;

public class ScaleTest
{
    private static readonly ScaleDb TestScaleDb = new(
        "ScaleIdentifier",
        "ScaleName",
        "ScaleImplementation"
    );
    private static readonly string BookooBleServiceId = "00000FFE-0000-1000-8000-00805F9B34FB";

    private static readonly string BookooBleCommandCharacteristicId =
        "0000FF12-0000-1000-8000-00805F9B34FB";

    private static readonly string BookooBleWeightDataCharacteristicId =
        "0000FF11-0000-1000-8000-00805F9B34FB";

    [Fact]
    public void CreateScaleTest()
    {
        var scale = new Scale(TestScaleDb, Mock.Of<IBluetoothService>());
        Assert.Equal("ScaleName", scale.Name);
    }

    [Fact]
    public void BookooBleServiceIdTest()
    {
        Assert.Single(Scale.RequiredServiceIds);
        Assert.Contains(BookooBleServiceId, Scale.RequiredServiceIds);
    }

    [Fact]
    public async Task ConnectScaleTest()
    {
        var bluetoothServiceMock = new Mock<IBluetoothService>();
        var bleConnectionMock = new Mock<IBleDeviceConnection>();
        var bleServiceMock = new Mock<IBleService>();
        var bleWeightDataCharacteristicMock = new Mock<IBleCharacteristic>();
        bluetoothServiceMock
            .Setup(m => m.ConnectDeviceAsync(TestScaleDb.Identifier, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(bleConnectionMock.Object));
        bleConnectionMock
            .Setup(m => m.GetServiceAsync(BookooBleServiceId, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(bleServiceMock.Object));
        bleServiceMock
            .Setup(m =>
                m.GetCharacteristicAsync(
                    BookooBleCommandCharacteristicId,
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.FromResult(Mock.Of<IBleCharacteristic>()));
        bleServiceMock
            .Setup(m =>
                m.GetCharacteristicAsync(
                    BookooBleWeightDataCharacteristicId,
                    It.IsAny<CancellationToken>()
                )
            )
            .Returns(Task.FromResult(bleWeightDataCharacteristicMock.Object));
        bleWeightDataCharacteristicMock
            .Setup(m => m.GetValueObservableAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Observable.Empty<byte[]>()));
        var scale = new Scale(TestScaleDb, bluetoothServiceMock.Object);
        await scale.ConnectAsync(CancellationToken.None);
        bluetoothServiceMock.Verify(m =>
            m.ConnectDeviceAsync(TestScaleDb.Identifier, It.IsAny<CancellationToken>())
        );
        bleConnectionMock.Verify(m =>
            m.GetServiceAsync(BookooBleServiceId, It.IsAny<CancellationToken>())
        );
        bleServiceMock.Verify(m =>
            m.GetCharacteristicAsync(
                BookooBleCommandCharacteristicId,
                It.IsAny<CancellationToken>()
            )
        );
        bleServiceMock.Verify(m =>
            m.GetCharacteristicAsync(
                BookooBleWeightDataCharacteristicId,
                It.IsAny<CancellationToken>()
            )
        );
        bleWeightDataCharacteristicMock.Verify(
            m => m.GetValueObservableAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
        bluetoothServiceMock.VerifyNoOtherCalls();
        bleConnectionMock.VerifyNoOtherCalls();
        bleServiceMock.VerifyNoOtherCalls();
        bleWeightDataCharacteristicMock.VerifyNoOtherCalls();
    }
}
