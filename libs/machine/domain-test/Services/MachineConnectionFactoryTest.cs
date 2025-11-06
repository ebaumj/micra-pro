using MicraPro.Machine.Domain.BluetoothAccess;
using MicraPro.Machine.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace MicraPro.Machine.Domain.Test.Services;

public class MachineConnectionFactoryTest
{
    [Fact]
    public async Task CreateAsyncTest()
    {
        var bluetoothServiceMock = new Mock<IBluetoothService>();
        var tokenCharacteristicMock = new Mock<IBluetoothCharacteristic>();
        tokenCharacteristicMock
            .Setup(m => m.ReadAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult("Some Token"));
        var authCharacteristicMock = new Mock<IBluetoothCharacteristic>();
        bluetoothServiceMock
            .Setup(m => m.ConnectAsync("Id", Is.Ct))
            .ReturnsAsync(
                Mock.Of<IBluetoothConnection>(m =>
                    m.Token == tokenCharacteristicMock.Object
                    && m.Auth == authCharacteristicMock.Object
                )
            );
        var factory = new MachineConnectionFactory(
            bluetoothServiceMock.Object,
            Mock.Of<ILogger<MachineConnection>>()
        );
        Assert.NotNull(await factory.CreateAsync("Id", Is.Ct));
        bluetoothServiceMock.Verify(m => m.ConnectAsync("Id", Is.Ct), Times.Once);
        tokenCharacteristicMock.Verify(m => m.ReadAsync(It.IsAny<CancellationToken>()), Times.Once);
        authCharacteristicMock.Verify(
            m => m.WriteAsync("Some Token", It.IsAny<CancellationToken>()),
            Times.Once
        );
        tokenCharacteristicMock.VerifyNoOtherCalls();
        authCharacteristicMock.VerifyNoOtherCalls();
        bluetoothServiceMock.VerifyNoOtherCalls();
    }
}
