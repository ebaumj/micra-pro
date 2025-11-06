using MicraPro.Machine.Domain.BluetoothAccess;
using MicraPro.Machine.Domain.Interfaces;
using MicraPro.Machine.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;

namespace MicraPro.Machine.Domain.Test.Services;

public class MachineConnectionTest
{
    [Fact]
    public void AllSettingsCoveredTest()
    {
        Assert.True(
            Enum.GetValues<IMachineConnection.ReadSetting>()
                .All(v => MachineConnection.SettingsDictionaryTest.Contains(v))
        );
    }

    [Theory]
    [InlineData(IMachineConnection.ReadSetting.MachineCapabilities, "machineCapabilities\0")]
    [InlineData(IMachineConnection.ReadSetting.MachineMode, "machineMode\0")]
    [InlineData(IMachineConnection.ReadSetting.TankStatus, "tankStatus\0")]
    [InlineData(IMachineConnection.ReadSetting.Boilers, "boilers\0")]
    [InlineData(IMachineConnection.ReadSetting.SmartStandBy, "smartStandBy\0")]
    public async Task ReadValueAsyncTest(
        IMachineConnection.ReadSetting setting,
        string settingValue
    )
    {
        var readCharacteristicMock = new Mock<IBluetoothCharacteristic>();
        readCharacteristicMock
            .Setup(m => m.ReadAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult("Some Value"));
        var service = new MachineConnection(
            Mock.Of<IBluetoothConnection>(m => m.Read == readCharacteristicMock.Object),
            Mock.Of<ILogger<MachineConnection>>()
        );
        var value = await service.ReadValueAsync(setting, CancellationToken.None);
        readCharacteristicMock.Verify(
            m => m.WriteAsync(settingValue, It.IsAny<CancellationToken>()),
            Times.Once
        );
        readCharacteristicMock.Verify(m => m.ReadAsync(It.IsAny<CancellationToken>()), Times.Once);
        readCharacteristicMock.VerifyNoOtherCalls();
        Assert.Equal("Some Value", value);
    }

    [Fact]
    public async Task AuthenticateAsyncTest()
    {
        var tokenCharacteristicMock = new Mock<IBluetoothCharacteristic>();
        tokenCharacteristicMock
            .Setup(m => m.ReadAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult("Some Token"));
        var authCharacteristicMock = new Mock<IBluetoothCharacteristic>();
        var service = new MachineConnection(
            Mock.Of<IBluetoothConnection>(m =>
                m.Token == tokenCharacteristicMock.Object && m.Auth == authCharacteristicMock.Object
            ),
            Mock.Of<ILogger<MachineConnection>>()
        );
        await service.AuthenticateAsync(CancellationToken.None);
        tokenCharacteristicMock.Verify(m => m.ReadAsync(It.IsAny<CancellationToken>()), Times.Once);
        authCharacteristicMock.Verify(
            m => m.WriteAsync("Some Token", It.IsAny<CancellationToken>()),
            Times.Once
        );
        tokenCharacteristicMock.VerifyNoOtherCalls();
        authCharacteristicMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task WriteValueAsyncTest()
    {
        var writeCharacteristicMock = new Mock<IBluetoothCharacteristic>();
        var service = new MachineConnection(
            Mock.Of<IBluetoothConnection>(m => m.Write == writeCharacteristicMock.Object),
            Mock.Of<ILogger<MachineConnection>>()
        );
        await service.WriteValueAsync(
            "Some Name",
            new { data = "Some Data", data2 = "Other Data" },
            CancellationToken.None
        );
        writeCharacteristicMock.Verify(
            m =>
                m.WriteAsync(
                    "{\"name\":\"Some Name\",\"parameter\":{\"data\":\"Some Data\",\"data2\":\"Other Data\"}}\0",
                    It.IsAny<CancellationToken>()
                ),
            Times.Once
        );
        writeCharacteristicMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DisconnectAsyncTest()
    {
        var btConnectionMock = new Mock<IBluetoothConnection>();
        btConnectionMock
            .Setup(m => m.DisconnectAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var service = new MachineConnection(
            btConnectionMock.Object,
            Mock.Of<ILogger<MachineConnection>>()
        );
        await service.DisconnectAsync(CancellationToken.None);
        btConnectionMock.Verify(m => m.DisconnectAsync(It.IsAny<CancellationToken>()), Times.Once);
        btConnectionMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task IsStandbyTest()
    {
        var readCharacteristicMock = new Mock<IBluetoothCharacteristic>();
        readCharacteristicMock
            .Setup(m => m.ReadAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult("StandBy"));
        var observerMock = new Mock<IObserver<bool>>();
        var service = new MachineConnection(
            Mock.Of<IBluetoothConnection>(m => m.Read == readCharacteristicMock.Object),
            Mock.Of<ILogger<MachineConnection>>()
        );
        service.IsStandby.Subscribe(observerMock.Object);
        service.SubscribeToStandby();
        observerMock.Verify(o => o.OnNext(false), Times.Once);
        await Task.Delay(TimeSpan.FromMilliseconds(300));
        observerMock.Verify(o => o.OnNext(true), Times.Once);
    }
}
