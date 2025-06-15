using System.Reactive.Linq;
using MicraPro.ScaleManagement.DataDefinition.ValueObjects;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using MicraPro.ScaleManagement.Domain.ScaleImplementations.BookooThemisMini;
using Moq;

namespace MicraPro.ScaleManagement.Domain.Test.ScaleImplementations.BookooTemisMini;

public class ScaleConnectionTest
{
    public static readonly TheoryData<(byte[] Input, ScaleDataPoint Output)> WeightTestData = new(
        (
            [
                0x03,
                0x0B,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00, // Weight
                0x00,
                0x00,
                0x00, // Flow
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x08,
            ], // Crc
            new ScaleDataPoint(DateTime.Now, 0, 0)
        ),
        (
            [
                0x03,
                0x0B,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x01,
                0x40,
                0x21, // Weight
                0x00,
                0x00,
                0x00, // Flow
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x68,
            ], // Crc
            new ScaleDataPoint(DateTime.Now, 0, 819.53)
        ),
        (
            [
                0x03,
                0x0B,
                0x00,
                0x00,
                0x00,
                0x00,
                0x2D,
                0x01,
                0x40,
                0x21, // Weight
                0x00,
                0x00,
                0x00, // Flow
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x45,
            ], // Crc
            new ScaleDataPoint(DateTime.Now, 0, -819.53)
        ),
        (
            [
                0x03,
                0x0B,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00, // Weight
                0x00,
                0x04,
                0xB7, // Flow
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0xBB,
            ], // Crc
            new ScaleDataPoint(DateTime.Now, 12.07, 0)
        ),
        (
            [
                0x03,
                0x0B,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00, // Weight
                0x2D,
                0x04,
                0xB7, // Flow
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x00,
                0x96,
            ], // Crc
            new ScaleDataPoint(DateTime.Now, -12.07, 0)
        )
    );

    [Fact]
    public async Task DisconnectTest()
    {
        var bleDeviceConnectionMock = new Mock<IBleDeviceConnection>();
        bleDeviceConnectionMock
            .Setup(m => m.Disconnect(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        await new ScaleConnection(
            Mock.Of<IBleCharacteristic>(),
            Mock.Of<IObservable<byte[]>>(),
            bleDeviceConnectionMock.Object
        ).DisconnectAsync(CancellationToken.None);
        bleDeviceConnectionMock.Verify(
            m => m.Disconnect(It.IsAny<CancellationToken>()),
            Times.Once
        );
        bleDeviceConnectionMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task TareTest()
    {
        byte[] dataBytes = [0x03, 0x0A, 0x01, 0x00, 0x00, 0x08];
        var bleCommandCharacteristicMock = new Mock<IBleCharacteristic>();
        bleCommandCharacteristicMock
            .Setup(m => m.SendCommandAsync(It.IsAny<byte[]>(), It.IsAny<CancellationToken>()))
            .Callback(
                (byte[] d, CancellationToken _) =>
                {
                    Assert.Equivalent(dataBytes, d);
                }
            )
            .Returns(Task.CompletedTask);
        await new ScaleConnection(
            bleCommandCharacteristicMock.Object,
            Mock.Of<IObservable<byte[]>>(),
            Mock.Of<IBleDeviceConnection>()
        ).TareAsync(CancellationToken.None);
        bleCommandCharacteristicMock.Verify(
            m => m.SendCommandAsync(It.IsAny<byte[]>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        bleCommandCharacteristicMock.VerifyNoOtherCalls();
    }

    [Theory]
    [MemberData(nameof(WeightTestData))]
    public void WeigthDataTest((byte[] Input, ScaleDataPoint Output) data)
    {
        var output = new ScaleConnection(
            Mock.Of<IBleCharacteristic>(),
            Observable.Return(data.Input),
            Mock.Of<IBleDeviceConnection>()
        )
            .Data.ToEnumerable()
            .ToArray();
        var sum = data.Input.Take(19).Aggregate(0, (acc, d) => acc ^ d);
        Assert.Single(output);
        Assert.Equal(data.Output.Weight, output[0].Weight);
        Assert.Equal(data.Output.Flow, output[0].Flow);
    }
}
