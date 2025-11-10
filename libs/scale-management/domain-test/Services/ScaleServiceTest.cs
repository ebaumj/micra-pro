using System.Reactive.Linq;
using System.Reactive.Subjects;
using MicraPro.ScaleManagement.DataDefinition;
using MicraPro.ScaleManagement.DataDefinition.ValueObjects;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using MicraPro.ScaleManagement.Domain.ScaleImplementations;
using MicraPro.ScaleManagement.Domain.Services;
using MicraPro.ScaleManagement.Domain.StorageAccess;
using Moq;

namespace MicraPro.ScaleManagement.Domain.Test.Services;

public class ScaleServiceTest
{
    private record Scale : IScale
    {
        public Task<IScaleConnection> ConnectAsync(CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }

    [Fact]
    public async Task ScanAsyncTest()
    {
        var bluetoothServiceMock = new Mock<IBluetoothService>();
        bluetoothServiceMock
            .Setup(m => m.DiscoverAsync(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var service = new ScaleService(
            bluetoothServiceMock.Object,
            Mock.Of<IScaleRepository>(),
            Mock.Of<IScaleImplementationCollectionService>(),
            new ScaleImplementationMemoryService()
        );
        await service.ScanAsync(TimeSpan.FromSeconds(5), CancellationToken.None);
        bluetoothServiceMock.Verify(
            m => m.DiscoverAsync(TimeSpan.FromSeconds(5), It.IsAny<CancellationToken>()),
            Times.Once
        );
        bluetoothServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void IsScanningTest()
    {
        var isScanningSubject = new Subject<bool>();
        var observerMock = new Mock<IObserver<bool>>();
        var bluetoothServiceMock = new Mock<IBluetoothService>();
        bluetoothServiceMock.Setup(m => m.IsScanning).Returns(isScanningSubject);
        var service = new ScaleService(
            bluetoothServiceMock.Object,
            Mock.Of<IScaleRepository>(),
            Mock.Of<IScaleImplementationCollectionService>(),
            new ScaleImplementationMemoryService()
        );
        service.IsScanning.Subscribe(observerMock.Object);
        bluetoothServiceMock.Verify(m => m.IsScanning, Times.Once);
        bluetoothServiceMock.VerifyNoOtherCalls();
        isScanningSubject.OnNext(false);
        observerMock.Verify(m => m.OnNext(false), Times.Once);
        isScanningSubject.OnNext(true);
        observerMock.Verify(m => m.OnNext(true), Times.Once);
        observerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void DetectedScalesTest()
    {
        BluetoothScanResult[] devices =
        [
            // Already in db
            new("Device 1", "Id1", ["service-id-1", "service-id-2"]),
            // Has not all the required services
            new("Device 2", "Id2", ["service-id-1", "service-id-3"]),
            // Gets Detected
            new("Device 3", "Id3", ["service-id-1", "service-id-2", "service-id-3"]),
        ];
        var bluetoothServiceMock = new Mock<IBluetoothService>();
        bluetoothServiceMock.Setup(m => m.DetectedDevices).Returns(devices.ToObservable());
        var bluetoothDeviceObserverMock = new Mock<IObserver<BluetoothScale>>();
        var scaleRepositoryMock = new Mock<IScaleRepository>();
        scaleRepositoryMock
            .Setup(m => m.GetScaleAsync(It.IsAny<CancellationToken>()))
            .Returns(() => Task.FromResult<ScaleDb?>(new ScaleDb("Id1", "")));
        var scaleImplementationCollectionServiceMock =
            new Mock<IScaleImplementationCollectionService>();
        scaleImplementationCollectionServiceMock
            .Setup(m => m.Implementations)
            .Returns([("Implementation", ["service-id-1", "service-id-2"])]);
        var memory = new ScaleImplementationMemoryService();
        var service = new ScaleService(
            bluetoothServiceMock.Object,
            scaleRepositoryMock.Object,
            scaleImplementationCollectionServiceMock.Object,
            memory
        );
        service.DetectedScales.Subscribe(bluetoothDeviceObserverMock.Object);
        bluetoothDeviceObserverMock.Verify(
            m => m.OnNext(new BluetoothScale("Device 3", "Id3")),
            Times.Once
        );
        bluetoothDeviceObserverMock.Verify(m => m.OnCompleted(), Times.Once);
        bluetoothDeviceObserverMock.VerifyNoOtherCalls();
        Assert.Equal("Implementation", memory.GetImplementation("Id3"));
        Assert.Equal("Implementation", memory.GetImplementation("Id1"));
        Assert.Throws<KeyNotFoundException>(() => memory.GetImplementation("Id2"));
    }

    [Fact]
    public async Task AddScaleAsyncTest()
    {
        var scaleRepositoryMock = new Mock<IScaleRepository>();
        scaleRepositoryMock
            .Setup(m => m.AddOrUpdateScaleAsync(It.IsAny<ScaleDb>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var scaleImplementationCollectionServiceMock =
            new Mock<IScaleImplementationCollectionService>();
        scaleImplementationCollectionServiceMock
            .Setup(m => m.CreateScale(It.IsAny<ScaleDb>()))
            .Returns((ScaleDb _) => new Scale());
        var memory = new ScaleImplementationMemoryService();
        var service = new ScaleService(
            Mock.Of<IBluetoothService>(),
            scaleRepositoryMock.Object,
            scaleImplementationCollectionServiceMock.Object,
            memory
        );
        memory.SetImplementation("Id", "Implementation");
        _ = await service.AddOrUpdateScaleAsync("Id", CancellationToken.None);
        scaleImplementationCollectionServiceMock.Verify(
            m => m.CreateScale(It.IsAny<ScaleDb>()),
            Times.Once
        );
        scaleRepositoryMock.Verify(
            m => m.AddOrUpdateScaleAsync(It.IsAny<ScaleDb>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        scaleImplementationCollectionServiceMock.VerifyNoOtherCalls();
        scaleRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RemoveScaleAsyncTest()
    {
        var scaleRepositoryMock = new Mock<IScaleRepository>();
        scaleRepositoryMock
            .Setup(m => m.DeleteScaleAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var service = new ScaleService(
            Mock.Of<IBluetoothService>(),
            scaleRepositoryMock.Object,
            Mock.Of<IScaleImplementationCollectionService>(),
            new ScaleImplementationMemoryService()
        );
        await service.RemoveScaleAsync(CancellationToken.None);
        scaleRepositoryMock.Verify(
            m => m.DeleteScaleAsync(It.IsAny<CancellationToken>()),
            Times.Once
        );
        scaleRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetScaleAsyncTest()
    {
        var scale1 = new ScaleDb("Id1", "Implementation");
        var scaleRepositoryMock = new Mock<IScaleRepository>();
        scaleRepositoryMock
            .Setup(m => m.GetScaleAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<ScaleDb?>(scale1));
        var scaleImplementationCollectionServiceMock =
            new Mock<IScaleImplementationCollectionService>();
        scaleImplementationCollectionServiceMock
            .Setup(m => m.CreateScale(It.IsAny<ScaleDb>()))
            .Returns((ScaleDb _) => new Scale());
        var memory = new ScaleImplementationMemoryService();
        var service = new ScaleService(
            Mock.Of<IBluetoothService>(),
            scaleRepositoryMock.Object,
            scaleImplementationCollectionServiceMock.Object,
            memory
        );
        var result = await service.GetScaleAsync(CancellationToken.None);
        Assert.NotNull(result);
        scaleRepositoryMock.Verify(m => m.GetScaleAsync(It.IsAny<CancellationToken>()), Times.Once);
        scaleImplementationCollectionServiceMock.Verify(m => m.CreateScale(scale1), Times.Once);
        scaleRepositoryMock.VerifyNoOtherCalls();
        scaleImplementationCollectionServiceMock.VerifyNoOtherCalls();
    }
}
