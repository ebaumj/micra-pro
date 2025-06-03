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
    private record Scale(Guid Id, string Name) : IScale
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
            Mock.Of<IScaleRespository>(),
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
            Mock.Of<IScaleRespository>(),
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
        var scaleRepositoryMock = new Mock<IScaleRespository>();
        scaleRepositoryMock
            .Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>()))
            .Returns(() =>
                Task.FromResult<IReadOnlyCollection<ScaleDb>>([new ScaleDb("Id1", "", "")])
            );
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
        var scaleRepositoryMock = new Mock<IScaleRespository>();
        scaleRepositoryMock
            .Setup(m => m.AddAsync(It.IsAny<ScaleDb>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        scaleRepositoryMock
            .Setup(m => m.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var scaleImplementationCollectionServiceMock =
            new Mock<IScaleImplementationCollectionService>();
        scaleImplementationCollectionServiceMock
            .Setup(m => m.CreateScale(It.IsAny<ScaleDb>()))
            .Returns((ScaleDb s) => new Scale(s.Id, s.Name));
        var memory = new ScaleImplementationMemoryService();
        var service = new ScaleService(
            Mock.Of<IBluetoothService>(),
            scaleRepositoryMock.Object,
            scaleImplementationCollectionServiceMock.Object,
            memory
        );
        memory.SetImplementation("Id", "Implementation");
        var scale = await service.AddScaleAsync("MyScale", "Id", CancellationToken.None);
        Assert.Equal("MyScale", scale.Name);
        scaleImplementationCollectionServiceMock.Verify(
            m => m.CreateScale(It.IsAny<ScaleDb>()),
            Times.Once
        );
        scaleRepositoryMock.Verify(m => m.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        scaleRepositoryMock.Verify(
            m => m.AddAsync(It.IsAny<ScaleDb>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        scaleImplementationCollectionServiceMock.VerifyNoOtherCalls();
        scaleRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RemoveScaleAsyncTest()
    {
        var scaleRepositoryMock = new Mock<IScaleRespository>();
        scaleRepositoryMock
            .Setup(m => m.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        scaleRepositoryMock
            .Setup(m => m.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var service = new ScaleService(
            Mock.Of<IBluetoothService>(),
            scaleRepositoryMock.Object,
            Mock.Of<IScaleImplementationCollectionService>(),
            new ScaleImplementationMemoryService()
        );
        var id = Guid.NewGuid();
        var result = await service.RemoveScaleAsync(id, CancellationToken.None);
        Assert.Equal(result, id);
        scaleRepositoryMock.Verify(
            m => m.DeleteAsync(id, It.IsAny<CancellationToken>()),
            Times.Once
        );
        scaleRepositoryMock.Verify(m => m.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        scaleRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetScalesAsyncTest()
    {
        var scale1 = new ScaleDb("Id1", "Name1", "Implementation");
        var scale2 = new ScaleDb("Id2", "Name2", "Implementation");
        var scaleRepositoryMock = new Mock<IScaleRespository>();
        scaleRepositoryMock
            .Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IReadOnlyCollection<ScaleDb>>([scale1, scale2]));
        var scaleImplementationCollectionServiceMock =
            new Mock<IScaleImplementationCollectionService>();
        scaleImplementationCollectionServiceMock
            .Setup(m => m.CreateScale(It.IsAny<ScaleDb>()))
            .Returns((ScaleDb s) => new Scale(s.Id, s.Name));
        var memory = new ScaleImplementationMemoryService();
        var service = new ScaleService(
            Mock.Of<IBluetoothService>(),
            scaleRepositoryMock.Object,
            scaleImplementationCollectionServiceMock.Object,
            memory
        );
        var result = (await service.GetScalesAsync(CancellationToken.None)).ToArray();
        Assert.Equal("Name1", result.FirstOrDefault(s => s.Id == scale1.Id)?.Name);
        Assert.Equal("Name2", result.FirstOrDefault(s => s.Id == scale2.Id)?.Name);
        scaleRepositoryMock.Verify(m => m.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        scaleImplementationCollectionServiceMock.Verify(m => m.CreateScale(scale1), Times.Once);
        scaleImplementationCollectionServiceMock.Verify(m => m.CreateScale(scale2), Times.Once);
        scaleRepositoryMock.VerifyNoOtherCalls();
        scaleImplementationCollectionServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetScaleAsyncTest()
    {
        var scale = new ScaleDb("Id1", "Name", "Implementation");
        var scaleRepositoryMock = new Mock<IScaleRespository>();
        scaleRepositoryMock
            .Setup(m => m.GetByIdAsync(scale.Id, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(scale));
        var scaleImplementationCollectionServiceMock =
            new Mock<IScaleImplementationCollectionService>();
        scaleImplementationCollectionServiceMock
            .Setup(m => m.CreateScale(scale))
            .Returns(new Scale(scale.Id, scale.Name));
        var memory = new ScaleImplementationMemoryService();
        var service = new ScaleService(
            Mock.Of<IBluetoothService>(),
            scaleRepositoryMock.Object,
            scaleImplementationCollectionServiceMock.Object,
            memory
        );
        var result = await service.GetScaleAsync(scale.Id, CancellationToken.None);
        Assert.Equal("Name", result.Name);
        Assert.Equal(scale.Id, result.Id);
        scaleRepositoryMock.Verify(
            m => m.GetByIdAsync(scale.Id, It.IsAny<CancellationToken>()),
            Times.Once
        );
        scaleImplementationCollectionServiceMock.Verify(m => m.CreateScale(scale), Times.Once);
        scaleRepositoryMock.VerifyNoOtherCalls();
        scaleImplementationCollectionServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RenameScaleAsyncTest()
    {
        var scale = new ScaleDb("Id1", "NewName", "Implementation");
        var scaleRepositoryMock = new Mock<IScaleRespository>();
        scaleRepositoryMock
            .Setup(m => m.UpdateNameAsync(scale.Id, "NewName", It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(scale));
        var scaleImplementationCollectionServiceMock =
            new Mock<IScaleImplementationCollectionService>();
        scaleImplementationCollectionServiceMock
            .Setup(m => m.CreateScale(scale))
            .Returns(new Scale(scale.Id, scale.Name));
        var memory = new ScaleImplementationMemoryService();
        var service = new ScaleService(
            Mock.Of<IBluetoothService>(),
            scaleRepositoryMock.Object,
            scaleImplementationCollectionServiceMock.Object,
            memory
        );
        var renamedScale = await service.RenameScaleAsync(
            scale.Id,
            "NewName",
            CancellationToken.None
        );
        Assert.Equal("NewName", renamedScale.Name);
        Assert.Equal(scale.Id, renamedScale.Id);
        scaleRepositoryMock.Verify(
            m => m.UpdateNameAsync(scale.Id, "NewName", It.IsAny<CancellationToken>()),
            Times.Once
        );
        scaleImplementationCollectionServiceMock.Verify(m => m.CreateScale(scale), Times.Once);
        scaleRepositoryMock.VerifyNoOtherCalls();
        scaleImplementationCollectionServiceMock.VerifyNoOtherCalls();
    }
}
