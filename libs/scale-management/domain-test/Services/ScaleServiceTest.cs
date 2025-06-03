using System.Reactive.Linq;
using MicraPro.ScaleManagement.DataDefinition;
using MicraPro.ScaleManagement.Domain.BluetoothAccess;
using MicraPro.ScaleManagement.Domain.ScaleImplementations;
using MicraPro.ScaleManagement.Domain.Services;
using MicraPro.ScaleManagement.Domain.StorageAccess;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using Moq;

namespace MicraPro.ScaleManagement.Domain.Test.Services;

public class ScaleServiceTest
{
    private record NotImplementedScale(ScaleDb Value) : IScale
    {
        public Guid Id => Value.Id;
        public string Name => Value.Name;
        public IObservable<bool> IsAvailable => throw new NotImplementedException();

        public Task<IScaleConnection> Connect(CancellationToken ct) =>
            throw new NotImplementedException();
    }

    private class CacheEntryDummy() : ICacheEntry
    {
        public void Dispose() { }

        public object Key => throw new NotImplementedException();
        public object? Value { get; set; }
        public DateTimeOffset? AbsoluteExpiration
        {
            get => throw new NotImplementedException();
            set { }
        }
        public TimeSpan? AbsoluteExpirationRelativeToNow
        {
            get => throw new NotImplementedException();
            set { }
        }
        public TimeSpan? SlidingExpiration
        {
            get => throw new NotImplementedException();
            set { }
        }
        public IList<IChangeToken> ExpirationTokens => throw new NotImplementedException();
        public IList<PostEvictionCallbackRegistration> PostEvictionCallbacks =>
            throw new NotImplementedException();
        public CacheItemPriority Priority
        {
            get => throw new NotImplementedException();
            set { }
        }
        public long? Size
        {
            get => throw new NotImplementedException();
            set { }
        }
    }

    private record PresetValueCache(string Value) : IMemoryCache
    {
        public void Dispose() => throw new NotImplementedException();

        public bool TryGetValue(object key, out object? value)
        {
            value = Value;
            return true;
        }

        public ICacheEntry CreateEntry(object key) => throw new NotImplementedException();

        public void Remove(object key) => throw new NotImplementedException();
    }

    [Fact]
    public async Task ScanTest()
    {
        Guid[] serviceIds1 = [Guid.NewGuid()];
        Guid[] serviceIds2 = [Guid.NewGuid()];
        var bluetoothServiceMock = new Mock<IBluetoothService>();
        bluetoothServiceMock
            .Setup(m => m.ScanDevicesAsync(serviceIds1, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<string[]>(["Device 1", "Device 2", "Device 3"]));
        bluetoothServiceMock
            .Setup(m => m.ScanDevicesAsync(serviceIds2, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<string[]>(["Device 4"]));
        var scaleRepositoryMock = new Mock<IScaleRespository>();
        scaleRepositoryMock
            .Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>()))
            .Returns(
                Task.FromResult<IReadOnlyCollection<ScaleDb>>(
                    [new ScaleDb("Device 3", "Name", "Implementation")]
                )
            );
        var scaleImplementationCollectionServiceMock =
            new Mock<IScaleImplementationCollectionService>();
        scaleImplementationCollectionServiceMock
            .Setup(m => m.Implementations)
            .Returns([("implementation1", serviceIds1), ("implementation2", serviceIds2)]);
        var cacheMock = new Mock<IMemoryCache>();
        cacheMock.Setup(m => m.CreateEntry(It.IsAny<string>())).Returns(new CacheEntryDummy());
        var service = new ScaleService(
            bluetoothServiceMock.Object,
            scaleRepositoryMock.Object,
            scaleImplementationCollectionServiceMock.Object,
            cacheMock.Object
        );
        var scannedDevices = (await service.Scan(CancellationToken.None)).ToArray();
        Assert.Equal(3, scannedDevices.Length);
        Assert.Contains("Device 1", scannedDevices);
        Assert.Contains("Device 2", scannedDevices);
        Assert.Contains("Device 4", scannedDevices);
        Assert.DoesNotContain("Device 3", scannedDevices);
    }

    [Fact]
    public async Task GetScaleTest()
    {
        var scaleObject = new ScaleDb("SomeIdentifier", "Name", "SomeImplementation");
        var id = scaleObject.Id;
        var scaleImplementationCollectionServiceMock =
            new Mock<IScaleImplementationCollectionService>();
        scaleImplementationCollectionServiceMock
            .Setup(m => m.CreateScale(scaleObject))
            .Returns(new NotImplementedScale(scaleObject));
        var scaleRepositoryMock = new Mock<IScaleRespository>();
        scaleRepositoryMock
            .Setup(m => m.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(scaleObject));
        var service = new ScaleService(
            Mock.Of<IBluetoothService>(),
            scaleRepositoryMock.Object,
            scaleImplementationCollectionServiceMock.Object,
            Mock.Of<IMemoryCache>()
        );
        var scaleImplementation = await service.GetScale(id, CancellationToken.None);
        Assert.Equal("Name", scaleImplementation.Name);
        Assert.Equal(id, scaleImplementation.Id);
        scaleImplementationCollectionServiceMock.Verify(
            m => m.CreateScale(scaleObject),
            Times.Once
        );
        scaleRepositoryMock.Verify(
            m => m.GetByIdAsync(id, It.IsAny<CancellationToken>()),
            Times.Once
        );
        scaleImplementationCollectionServiceMock.VerifyNoOtherCalls();
        scaleRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetScalesTest()
    {
        var scaleObject1 = new ScaleDb("SomeIdentifier", "Name1", "SomeImplementation");
        var scaleObject2 = new ScaleDb("SomeIdentifier", "Name2", "SomeImplementation");
        var scaleImplementationCollectionServiceMock =
            new Mock<IScaleImplementationCollectionService>();
        scaleImplementationCollectionServiceMock
            .Setup(m => m.CreateScale(scaleObject1))
            .Returns(new NotImplementedScale(scaleObject1));
        scaleImplementationCollectionServiceMock
            .Setup(m => m.CreateScale(scaleObject2))
            .Returns(new NotImplementedScale(scaleObject2));
        var scaleRepositoryMock = new Mock<IScaleRespository>();
        scaleRepositoryMock
            .Setup(m => m.GetAllAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult<IReadOnlyCollection<ScaleDb>>([scaleObject1, scaleObject2]));
        var service = new ScaleService(
            Mock.Of<IBluetoothService>(),
            scaleRepositoryMock.Object,
            scaleImplementationCollectionServiceMock.Object,
            Mock.Of<IMemoryCache>()
        );
        var scaleImplementations = (await service.GetScales(CancellationToken.None)).ToArray();
        Assert.Equal("Name1", scaleImplementations[0].Name);
        Assert.Equal("Name2", scaleImplementations[1].Name);
        Assert.Equal(scaleObject1.Id, scaleImplementations[0].Id);
        Assert.Equal(scaleObject2.Id, scaleImplementations[1].Id);
        scaleImplementationCollectionServiceMock.Verify(
            m => m.CreateScale(scaleObject1),
            Times.Once
        );
        scaleImplementationCollectionServiceMock.Verify(
            m => m.CreateScale(scaleObject2),
            Times.Once
        );
        scaleRepositoryMock.Verify(m => m.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        scaleImplementationCollectionServiceMock.VerifyNoOtherCalls();
        scaleRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task AddScaleTest()
    {
        var scaleImplementationCollectionServiceMock =
            new Mock<IScaleImplementationCollectionService>();
        scaleImplementationCollectionServiceMock
            .Setup(m => m.CreateScale(It.IsAny<ScaleDb>()))
            .Returns((ScaleDb s) => new NotImplementedScale(s));
        var scaleRepositoryMock = new Mock<IScaleRespository>();
        scaleRepositoryMock
            .Setup(m => m.AddAsync(It.IsAny<ScaleDb>(), It.IsAny<CancellationToken>()))
            .Callback(
                (ScaleDb s, CancellationToken _) =>
                {
                    Assert.Equal("Name", s.Name);
                    Assert.Equal("SomeIdentifier", s.Identifier);
                    Assert.Equal("SomeImplementation", s.ImplementationType);
                }
            )
            .Returns(Task.CompletedTask);
        scaleRepositoryMock
            .Setup(m => m.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var service = new ScaleService(
            Mock.Of<IBluetoothService>(),
            scaleRepositoryMock.Object,
            scaleImplementationCollectionServiceMock.Object,
            new PresetValueCache("SomeImplementation")
        );
        var scale = await service.AddScale("Name", "SomeIdentifier", CancellationToken.None);
        Assert.Equal("Name", scale.Name);
        scaleImplementationCollectionServiceMock.Verify(
            m => m.CreateScale(It.IsAny<ScaleDb>()),
            Times.Once
        );
        scaleRepositoryMock.Verify(
            m => m.AddAsync(It.IsAny<ScaleDb>(), It.IsAny<CancellationToken>()),
            Times.Once
        );
        scaleRepositoryMock.Verify(m => m.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        scaleImplementationCollectionServiceMock.VerifyNoOtherCalls();
        scaleRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RenameScaleTest()
    {
        var scaleObject = new ScaleDb("SomeIdentifier", "Name", "SomeImplementation");
        var id = scaleObject.Id;
        var scaleImplementationCollectionServiceMock =
            new Mock<IScaleImplementationCollectionService>();
        scaleImplementationCollectionServiceMock
            .Setup(m => m.CreateScale(scaleObject))
            .Returns(new NotImplementedScale(scaleObject));
        var scaleRepositoryMock = new Mock<IScaleRespository>();
        scaleRepositoryMock
            .Setup(m => m.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(scaleObject));
        scaleRepositoryMock
            .Setup(m => m.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        scaleRepositoryMock
            .Setup(m => m.UpdateName(id, It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(
                (Guid _, string name, CancellationToken _) =>
                {
                    scaleObject.Name = name;
                    return Task.FromResult(scaleObject);
                }
            );
        var service = new ScaleService(
            Mock.Of<IBluetoothService>(),
            scaleRepositoryMock.Object,
            scaleImplementationCollectionServiceMock.Object,
            Mock.Of<IMemoryCache>()
        );
        var scaleImplementation = await service.RenameScale(id, "NewName", CancellationToken.None);
        Assert.Equal("NewName", scaleImplementation.Name);
        scaleImplementationCollectionServiceMock.Verify(
            m => m.CreateScale(scaleObject),
            Times.Once
        );
        scaleRepositoryMock.Verify(
            m => m.UpdateName(id, "NewName", It.IsAny<CancellationToken>()),
            Times.Once
        );
        scaleImplementationCollectionServiceMock.VerifyNoOtherCalls();
        scaleRepositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RemoveScaleTest()
    {
        var id = Guid.NewGuid();
        var scaleRepositoryMock = new Mock<IScaleRespository>();
        scaleRepositoryMock
            .Setup(m => m.DeleteAsync(id, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        scaleRepositoryMock
            .Setup(m => m.SaveAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var service = new ScaleService(
            Mock.Of<IBluetoothService>(),
            scaleRepositoryMock.Object,
            Mock.Of<IScaleImplementationCollectionService>(),
            Mock.Of<IMemoryCache>()
        );
        var idReturned = await service.RemoveScale(id, CancellationToken.None);
        Assert.Equal(id, idReturned);
        scaleRepositoryMock.Verify(
            m => m.DeleteAsync(id, It.IsAny<CancellationToken>()),
            Times.Once
        );
        scaleRepositoryMock.Verify(m => m.SaveAsync(It.IsAny<CancellationToken>()), Times.Once);
        scaleRepositoryMock.VerifyNoOtherCalls();
    }
}
