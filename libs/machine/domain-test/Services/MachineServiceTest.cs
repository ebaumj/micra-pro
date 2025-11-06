using System.Reactive.Linq;
using System.Reactive.Subjects;
using MicraPro.Machine.DataDefinition;
using MicraPro.Machine.Domain.BluetoothAccess;
using MicraPro.Machine.Domain.DatabaseAccess;
using MicraPro.Machine.Domain.Interfaces;
using MicraPro.Machine.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace MicraPro.Machine.Domain.Test.Services;

public class MachineServiceTest
{
    [Fact]
    public void ScanTest()
    {
        var ct = new CancellationTokenSource();
        var isScanning = new BehaviorSubject<bool>(false);
        var bluetoothServiceMock = new Mock<IBluetoothService>();
        bluetoothServiceMock
            .Setup(m => m.DetectedDevices)
            .Returns(
                Observable
                    .Return<IBluetoothService.ScanResult[]>(
                        [
                            new IBluetoothService.ScanResult("Device1", "1"),
                            new IBluetoothService.ScanResult("Device2", "2"),
                        ]
                    )
                    .SelectMany(r => r)
            );
        bluetoothServiceMock.Setup(m => m.IsScanning).Returns(isScanning);
        bluetoothServiceMock
            .Setup(m => m.DiscoverAsync(It.IsAny<TimeSpan>(), Is.Ct))
            .Callback(() => isScanning.OnNext(true))
            .Returns(Task.CompletedTask);
        var observerMock = new Mock<IObserver<IMachineService.MachineScanResult>>();
        var service = new MachineService(
            bluetoothServiceMock.Object,
            Mock.Of<IServiceScopeFactory>(),
            Mock.Of<IMachineConnectionFactory>(),
            Mock.Of<IMachineFactory>(),
            Mock.Of<ILogger<MachineService>>()
        );
        // Scan stop automatically
        service.Scan(CancellationToken.None).Subscribe(observerMock.Object);
        bluetoothServiceMock.Verify(m => m.DetectedDevices, Times.Once);
        bluetoothServiceMock.Verify(m => m.IsScanning, Times.Once);
        bluetoothServiceMock.Verify(
            m => m.DiscoverAsync(TimeSpan.FromSeconds(30), Is.Ct),
            Times.Once
        );
        Assert.True(isScanning.Value);
        observerMock.Verify(
            m => m.OnNext(new IMachineService.MachineScanResult("Device1", "1")),
            Times.Once
        );
        observerMock.Verify(
            m => m.OnNext(new IMachineService.MachineScanResult("Device2", "2")),
            Times.Once
        );
        isScanning.OnNext(false);
        observerMock.Verify(m => m.OnCompleted(), Times.Once);
        // Scan stop manually
        service.Scan(ct.Token).Subscribe(observerMock.Object);
        bluetoothServiceMock.Verify(m => m.DetectedDevices, Times.Exactly(2));
        bluetoothServiceMock.Verify(m => m.IsScanning, Times.Exactly(2));
        bluetoothServiceMock.Verify(
            m => m.DiscoverAsync(TimeSpan.FromSeconds(30), Is.Ct),
            Times.Exactly(2)
        );
        Assert.True(isScanning.Value);
        observerMock.Verify(
            m => m.OnNext(new IMachineService.MachineScanResult("Device1", "1")),
            Times.Exactly(2)
        );
        observerMock.Verify(
            m => m.OnNext(new IMachineService.MachineScanResult("Device2", "2")),
            Times.Exactly(2)
        );
        ct.Cancel();
        observerMock.Verify(m => m.OnCompleted(), Times.Exactly(2));
        bluetoothServiceMock.VerifyNoOtherCalls();
        observerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ConnectAsyncTest()
    {
        var machineConnectionFactoryMock = new Mock<IMachineConnectionFactory>();
        var machineConnection = Mock.Of<IMachineConnection>();
        var machineConnection2 = Mock.Of<IMachineConnection>();
        machineConnectionFactoryMock
            .Setup(m => m.CreateAsync("Id", Is.Ct))
            .Returns(Task.FromResult(machineConnection));
        machineConnectionFactoryMock
            .Setup(m => m.CreateAsync("Id2", Is.Ct))
            .Returns(Task.FromResult(machineConnection2));
        machineConnectionFactoryMock
            .Setup(m => m.CreateAsync("Id3", Is.Ct))
            .Throws(new Exception());
        var machineFactoryMock = new Mock<IMachineFactory>();
        var machine = Mock.Of<IMachine>();
        var machine2 = Mock.Of<IMachine>();
        machineFactoryMock.Setup(m => m.Create(machineConnection)).Returns(machine);
        machineFactoryMock.Setup(m => m.Create(machineConnection2)).Returns(machine2);
        var repositoryMock = new Mock<IMachineRepository>();
        repositoryMock
            .Setup(m => m.GetCurrentMachineAsync(Is.Ct))
            .Returns(Task.FromResult<string?>("Id"));
        var observerMock = new Mock<IObserver<IMachine?>>();
        var service = new MachineService(
            Mock.Of<IBluetoothService>(),
            Mock.Of<IServiceScopeFactory>(m =>
                m.CreateScope()
                == Mock.Of<IServiceScope>(sc =>
                    sc.ServiceProvider
                    == Mock.Of<IServiceProvider>(sp =>
                        sp.GetService(typeof(IMachineRepository)) == repositoryMock.Object
                    )
                )
            ),
            machineConnectionFactoryMock.Object,
            machineFactoryMock.Object,
            Mock.Of<ILogger<MachineService>>()
        );
        service.MachineObservable.Subscribe(observerMock.Object);
        Assert.Null(service.Machine);
        observerMock.Verify(m => m.OnNext(null));
        await service.ConnectAsync("Id", CancellationToken.None);
        machineConnectionFactoryMock.Verify(m => m.CreateAsync("Id", Is.Ct), Times.Once);
        Assert.Equal(machine, service.Machine);
        observerMock.Verify(m => m.OnNext(machine));
        machineFactoryMock.Verify(m => m.Create(machineConnection), Times.Exactly(2));
        repositoryMock.Verify(m => m.GetCurrentMachineAsync(Is.Ct), Times.Once);
        repositoryMock.Verify(m => m.SetCurrentMachineAsync("Id", Is.Ct), Times.Never);
        await service.ConnectAsync("Id2", CancellationToken.None);
        machineConnectionFactoryMock.Verify(m => m.CreateAsync("Id2", Is.Ct), Times.Once);
        Assert.Equal(machine2, service.Machine);
        observerMock.Verify(m => m.OnNext(machine2));
        machineFactoryMock.Verify(m => m.Create(machineConnection2), Times.Exactly(2));
        repositoryMock.Verify(m => m.GetCurrentMachineAsync(Is.Ct), Times.Exactly(2));
        repositoryMock.Verify(m => m.SetCurrentMachineAsync("Id2", Is.Ct), Times.Once);
        await Assert.ThrowsAsync<Exception>(() =>
            service.ConnectAsync("Id3", CancellationToken.None)
        );
        machineConnectionFactoryMock.Verify(m => m.CreateAsync("Id3", Is.Ct), Times.Once);
        repositoryMock.Verify(m => m.GetCurrentMachineAsync(Is.Ct), Times.Exactly(3));
        repositoryMock.Verify(m => m.SetCurrentMachineAsync("Id3", Is.Ct), Times.Once);
        machineConnectionFactoryMock.VerifyNoOtherCalls();
        machineFactoryMock.VerifyNoOtherCalls();
        repositoryMock.VerifyNoOtherCalls();
        observerMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DisconnectAsyncTest()
    {
        var machineConnectionFactoryMock = new Mock<IMachineConnectionFactory>();
        var machineConnectionMock = new Mock<IMachineConnection>();
        machineConnectionFactoryMock
            .Setup(m => m.CreateAsync("Id", Is.Ct))
            .Returns(Task.FromResult(machineConnectionMock.Object));
        var repositoryMock = new Mock<IMachineRepository>();
        repositoryMock
            .Setup(m => m.GetCurrentMachineAsync(Is.Ct))
            .Returns(Task.FromResult<string?>(null));
        var service = new MachineService(
            Mock.Of<IBluetoothService>(),
            Mock.Of<IServiceScopeFactory>(m =>
                m.CreateScope()
                == Mock.Of<IServiceScope>(sc =>
                    sc.ServiceProvider
                    == Mock.Of<IServiceProvider>(sp =>
                        sp.GetService(typeof(IMachineRepository)) == repositoryMock.Object
                    )
                )
            ),
            machineConnectionFactoryMock.Object,
            Mock.Of<IMachineFactory>(m =>
                m.Create(machineConnectionMock.Object) == Mock.Of<IMachine>()
            ),
            Mock.Of<ILogger<MachineService>>()
        );
        Assert.Null(service.Machine);
        await service.ConnectAsync("Id", CancellationToken.None);
        machineConnectionFactoryMock.Verify(m => m.CreateAsync("Id", Is.Ct), Times.Once);
        repositoryMock.Verify(m => m.GetCurrentMachineAsync(Is.Ct), Times.Once);
        repositoryMock.Verify(m => m.SetCurrentMachineAsync("Id", Is.Ct), Times.Once);
        Assert.NotNull(service.Machine);
        await service.DisconnectAsync(CancellationToken.None);
        repositoryMock.Verify(m => m.RemoveCurrentMachineAsync(Is.Ct), Times.Once);
        machineConnectionMock.Verify(m => m.DisconnectAsync(Is.Ct), Times.Once);
        Assert.Null(service.Machine);
        machineConnectionFactoryMock.VerifyNoOtherCalls();
        machineConnectionMock.VerifyNoOtherCalls();
        repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task StartAsyncTest()
    {
        var repositoryMock = new Mock<IMachineRepository>();
        repositoryMock
            .Setup(m => m.GetCurrentMachineAsync(Is.Ct))
            .Returns(Task.FromResult<string?>(null));
        var machineConnectionFactoryMock = new Mock<IMachineConnectionFactory>();
        var machineConnection = Mock.Of<IMachineConnection>();
        machineConnectionFactoryMock
            .Setup(m => m.CreateAsync("Id", Is.Ct))
            .Returns(Task.FromResult(machineConnection));
        var service = new MachineService(
            Mock.Of<IBluetoothService>(),
            Mock.Of<IServiceScopeFactory>(m =>
                m.CreateScope()
                == Mock.Of<IServiceScope>(sc =>
                    sc.ServiceProvider
                    == Mock.Of<IServiceProvider>(sp =>
                        sp.GetService(typeof(IMachineRepository)) == repositoryMock.Object
                    )
                )
            ),
            machineConnectionFactoryMock.Object,
            Mock.Of<IMachineFactory>(m => m.Create(machineConnection) == Mock.Of<IMachine>()),
            Mock.Of<ILogger<MachineService>>()
        );
        await service.StartAsync(CancellationToken.None);
        repositoryMock.Verify(m => m.GetCurrentMachineAsync(Is.Ct), Times.Once);
        Assert.Null(service.Machine);
        repositoryMock
            .Setup(m => m.GetCurrentMachineAsync(Is.Ct))
            .Returns(Task.FromResult<string?>("Id"));
        await service.StartAsync(CancellationToken.None);
        repositoryMock.Verify(m => m.GetCurrentMachineAsync(Is.Ct), Times.Exactly(2));
        machineConnectionFactoryMock.Verify(m => m.CreateAsync("Id", Is.Ct), Times.Once);
        Assert.NotNull(service.Machine);
        repositoryMock.VerifyNoOtherCalls();
        machineConnectionFactoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task StopAsyncTest()
    {
        var machineConnectionFactoryMock = new Mock<IMachineConnectionFactory>();
        var machineConnectionMock = new Mock<IMachineConnection>();
        machineConnectionFactoryMock
            .Setup(m => m.CreateAsync("Id", Is.Ct))
            .Returns(Task.FromResult(machineConnectionMock.Object));
        var repositoryMock = new Mock<IMachineRepository>();
        repositoryMock
            .Setup(m => m.GetCurrentMachineAsync(Is.Ct))
            .Returns(Task.FromResult<string?>(null));
        var service = new MachineService(
            Mock.Of<IBluetoothService>(),
            Mock.Of<IServiceScopeFactory>(m =>
                m.CreateScope()
                == Mock.Of<IServiceScope>(sc =>
                    sc.ServiceProvider
                    == Mock.Of<IServiceProvider>(sp =>
                        sp.GetService(typeof(IMachineRepository)) == repositoryMock.Object
                    )
                )
            ),
            machineConnectionFactoryMock.Object,
            Mock.Of<IMachineFactory>(m =>
                m.Create(machineConnectionMock.Object) == Mock.Of<IMachine>()
            ),
            Mock.Of<ILogger<MachineService>>()
        );
        Assert.Null(service.Machine);
        await service.ConnectAsync("Id", CancellationToken.None);
        machineConnectionFactoryMock.Verify(m => m.CreateAsync("Id", Is.Ct), Times.Once);
        repositoryMock.Verify(m => m.GetCurrentMachineAsync(Is.Ct), Times.Once);
        repositoryMock.Verify(m => m.SetCurrentMachineAsync("Id", Is.Ct), Times.Once);
        Assert.NotNull(service.Machine);
        await service.StopAsync(CancellationToken.None);
        machineConnectionMock.Verify(m => m.DisconnectAsync(Is.Ct), Times.Once);
        Assert.Null(service.Machine);
        machineConnectionFactoryMock.VerifyNoOtherCalls();
        machineConnectionMock.VerifyNoOtherCalls();
        repositoryMock.VerifyNoOtherCalls();
    }
}
